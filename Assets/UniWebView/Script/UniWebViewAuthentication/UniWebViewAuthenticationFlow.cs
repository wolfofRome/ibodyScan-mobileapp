﻿//
//  UniWebViewAuthenticationFlow.cs
//  Created by Wang Wei (@onevcat) on 2022-06-25.
//
//  This file is a part of UniWebView Project (https://uniwebview.com)
//  By purchasing the asset, you are allowed to use this code in as many as projects 
//  you want, only if you publish the final products under the name of the same account
//  used for the purchase. 
//
//  This asset and all corresponding files (such as source code) are provided on an 
//  窶彗s is窶?basis, without warranty of any kind, express of implied, including but not
//  limited to the warranties of merchantability, fitness for a particular purpose, and 
//  noninfringement. In no event shall the authors or copyright holders be liable for any 
//  claim, damages or other liability, whether in action of contract, tort or otherwise, 
//  arising from, out of or in connection with the software or the use of other dealing in the software.
//

using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interface for implementing a custom authentication flow. An authentication flow, in UniWebView, usually a "code" based
/// OAuth 2.0 flow, contains a standard set of steps:
/// 
/// 1. User is navigated to a web page that requires authentication;
/// 2. A temporary code is generated by service provider and provided to client by a redirect URL with customized scheme.
/// 3. Client requests an access token using the temporary code by performing an "access token" exchange request.
///
/// To use the common flow, any customize authentication flow must implement this interface and becomes a subclass of
/// UniWebViewAuthenticationCommonFlow.
/// </summary>
/// <typeparam name="TTokenType"></typeparam>
public interface IUniWebViewAuthenticationFlow<TTokenType>
{
    /// <summary>
    /// Returns the redirect URL that is used to redirect the user after authenticated. This is used as the `redirect_uri`
    /// parameter when navigating user to the authentication page.
    ///
    /// Usually this is a URL with customize scheme that later service provider may call. It takes intermediate code in its
    /// query and can be used to open the current app in client. The native side of UniWebView will catch and handle it,
    /// then send it to Unity side as the result of `UniWebViewAuthenticationSession`.
    /// </summary>
    /// <returns>
    /// The redirect URL set in the OAuth settings.
    /// </returns>
    string GetCallbackUrl();
    
    /// <summary>
    /// Returns the config of the authentication flow. It usually defines the authentication requests entry points.
    /// </summary>
    /// <returns>The config object of an authentication flow.</returns>
    UniWebViewAuthenticationConfiguration GetAuthenticationConfiguration();
    
    /// <summary>
    /// Returns a dictionary contains the parameters that are used to perform the authentication request.
    /// The key value pairs in the dictionary are used to construct the query string of the authentication request.
    ///
    /// This usually contains fields like `client_id`, `redirect_uri`, `response_type`, etc.
    /// </summary>
    /// <returns>The dictionary indicates parameters that are used to perform the authentication request.</returns>
    Dictionary<string, string> GetAuthenticationUriArguments();
    
    /// <summary>
    /// Returns a dictionary contains the parameters that are used to perform the access token exchange request.
    /// The key value pairs in the dictionary are used to construct the HTTP form body of the access token exchange request.
    /// </summary>
    /// <param name="authResponse">
    /// The response from authentication request. If the authentication succeeds, it is
    /// usually a custom scheme URL with a `code` query as its parameter. Base on this, you could construct the body of the
    /// access token exchange request.
    /// </param>
    /// <returns>
    /// The dictionary indicates parameters that are used to perform the access token exchange request.
    /// </returns>
    Dictionary<string, string> GetAccessTokenRequestParameters(string authResponse);

    /// <summary>
    /// Returns a dictionary contains the parameters that are used to perform the access token refresh request.
    /// The key value pairs in the dictionary are used to construct the HTTP form body of the access token refresh request.
    /// </summary>
    /// <param name="refreshToken">The refresh token should be used to perform the refresh request.</param>
    /// <returns>
    /// The dictionary indicates parameters that are used to perform the access token refresh request.
    /// </returns>
    Dictionary<string, string> GetRefreshTokenRequestParameters(string refreshToken);

    /// <summary>
    /// Returns the strong-typed token for the authentication process.
    ///
    /// When the token exchange request finishes without problem, the response body will be passed to this method and
    /// any conforming class should construct the token object from the response body.
    /// </summary>
    /// <param name="exchangeResponse">
    /// The body response of the access token exchange request. Usually it contains the desired `access_token` and other
    /// necessary fields to describe the authenticated result.
    /// </param>
    /// <returns>
    /// A token object with `TToken` type that represents the authenticated result.
    /// </returns>
    TTokenType GenerateTokenFromExchangeResponse(string exchangeResponse);
    
    /// <summary>
    /// Called when the authentication flow succeeds and a valid token is generated.
    /// </summary>
    UnityEvent<TTokenType> OnAuthenticationFinished { get; }

    /// <summary>
    /// Called when any error (including user cancellation) happens during the authentication flow.
    /// </summary>
    UnityEvent<long, string> OnAuthenticationErrored { get; }
    
    /// <summary>
    /// Called when the access token refresh request finishes and a valid refreshed token is generated.
    /// </summary>
    UnityEvent<TTokenType> OnRefreshTokenFinished { get; }

    /// <summary>
    /// Called when any error happens during the access token refresh flow.
    /// </summary>
    UnityEvent<long, string> OnRefreshTokenErrored { get; }
}

/// <summary>
/// The manager object of an authentication flow. This defines and runs the common flow of an authentication process
/// with `code` response type. 
/// </summary>
/// <typeparam name="TTokenType">The responsive token type expected for this authentication flow.</typeparam>
public class UniWebViewAuthenticationFlow<TTokenType> {
    
    private IUniWebViewAuthenticationFlow<TTokenType> service;

    public UniWebViewAuthenticationFlow(
        IUniWebViewAuthenticationFlow<TTokenType> service
    ) 
    {
        this.service = service;
    }

    /// <summary>
    /// Start the authentication flow.
    /// </summary>
    public void StartAuth()
    {
        var callbackUri = new Uri(service.GetCallbackUrl());
        var authUrl = GetAuthUrl();
        var session = UniWebViewAuthenticationSession.Create(authUrl, callbackUri.Scheme);
        var flow = service as UniWebViewAuthenticationCommonFlow;
        if (flow != null && flow.privateMode) {
            session.SetPrivateMode(true);
        }
        session.OnAuthenticationFinished += (_, resultUrl) =>  {
            UniWebViewLogger.Instance.Verbose("Auth flow received callback url: " + resultUrl);
            ExchangeToken(resultUrl);
        };

        session.OnAuthenticationErrorReceived += (_, errorCode, message) => {
            ExchangeTokenErrored(errorCode, message);
        };
        
        UniWebViewLogger.Instance.Verbose("Starting auth flow with url: " + authUrl + "; Callback scheme: " + callbackUri.Scheme);
        session.Start();
    }

    private void ExchangeToken(string response) {
        try {
            var args = service.GetAccessTokenRequestParameters(response);
            var request = GetTokenRequest(args);
            MonoBehaviour context = (MonoBehaviour)service;
            context.StartCoroutine(SendExchangeTokenRequest(request));
        } catch (Exception e) {
            var message = e.Message;
            var code = -1;
            if (e is AuthenticationResponseException ex) {
                code = ex.Code;
            }
            UniWebViewLogger.Instance.Critical("Exception on exchange token response: " + e + ". Code: " + code + ". Message: " + message);
            ExchangeTokenErrored(code, message);
        }
    }

    /// <summary>
    /// Refresh the access token with the given refresh token.
    /// </summary>
    /// <param name="refreshToken"></param>
    public void RefreshToken(string refreshToken) {
        try {
            var args = service.GetRefreshTokenRequestParameters(refreshToken);
            var request = GetTokenRequest(args);
            MonoBehaviour context = (MonoBehaviour)service;
            context.StartCoroutine(SendRefreshTokenRequest(request));
        } catch (Exception e) {
            var message = e.Message;
            var code = -1;
            if (e is AuthenticationResponseException ex) {
                code = ex.Code;
            }
            UniWebViewLogger.Instance.Critical("Exception on refresh token response: " + e + ". Code: " + code + ". Message: " + message);
            RefreshTokenErrored(code, message);
        }
    }

    private string GetAuthUrl() {
        var builder = new UriBuilder(service.GetAuthenticationConfiguration().authorizationEndpoint);
        var query = new NameValueCollection();
        foreach (var kv in service.GetAuthenticationUriArguments()) {
            query.Add(kv.Key, kv.Value);
        }
        builder.Query = UniWebViewAuthenticationUtils.CreateQueryString(query);
        return builder.ToString();
    }

    private UnityWebRequest GetTokenRequest(Dictionary<string, string>args) {
        var builder = new UriBuilder(service.GetAuthenticationConfiguration().tokenEndpoint);
        var form = new WWWForm();
        foreach (var kv in args) {
            form.AddField(kv.Key, kv.Value);
        }
        return UnityWebRequest.Post(builder.ToString(), form);
    }
    
    private IEnumerator SendExchangeTokenRequest(UnityWebRequest request) {
        return SendTokenRequest(request, ExchangeTokenFinished, ExchangeTokenErrored);
    }

    private IEnumerator SendRefreshTokenRequest(UnityWebRequest request) {
        return SendTokenRequest(request, RefreshTokenFinished, RefreshTokenErrored);
    }

    private IEnumerator SendTokenRequest(UnityWebRequest request, Action<TTokenType> finishAction, Action<long, string>errorAction) {
        using (var www = request) {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) {
                string errorMessage = null;
                string errorBody = null;
                if (www.error != null) {
                    errorMessage = www.error;
                }
                if (www.downloadHandler != null && www.downloadHandler.text != null) {
                    errorBody = www.downloadHandler.text;
                }
                UniWebViewLogger.Instance.Critical("Failed to get access token. Error: " + errorMessage + ". " + errorBody);
                errorAction(www.responseCode, errorBody ?? errorMessage);
            } else {
                var responseText = www.downloadHandler.text;
                UniWebViewLogger.Instance.Info("Token exchange request succeeded. Response: " + responseText);
                try {
                    var token = service.GenerateTokenFromExchangeResponse(www.downloadHandler.text);
                    finishAction(token);
                } catch (Exception e) {
                    var message = e.Message;
                    var code = -1;
                    if (e is AuthenticationResponseException ex) {
                        code = ex.Code;
                    }
                    UniWebViewLogger.Instance.Critical(
                        "Exception on parsing token response: " + e + ". Code: " + code + ". Message: " + 
                        message + ". Response: " + responseText);
                    errorAction(code, message);
                }
            }
        }
    }

    private void ExchangeTokenFinished(TTokenType token) {
        if (service.OnAuthenticationFinished != null) {
            service.OnAuthenticationFinished.Invoke(token);            
        }
        service = null;
    }
    
    private void ExchangeTokenErrored(long code, string message) {
        UniWebViewLogger.Instance.Info("Auth flow errored: " + code + ". Detail: " + message);
        if (service.OnAuthenticationErrored != null) {
            service.OnAuthenticationErrored.Invoke(code, message);
        }
        service = null;
    }
    
    private void RefreshTokenFinished(TTokenType token) {
        if (service.OnRefreshTokenFinished != null) {
            service.OnRefreshTokenFinished.Invoke(token);
        }
        service = null;
    }
    
    private void RefreshTokenErrored(long code, string message) {
        UniWebViewLogger.Instance.Info("Refresh flow errored: " + code + ". Detail: " + message);
        if (service.OnRefreshTokenErrored != null) {
            service.OnRefreshTokenErrored.Invoke(code, message);
        }
        service = null;
    }
}

/// <summary>
/// The configuration object of an authentication flow. This defines the authentication entry points.
/// </summary>
public class UniWebViewAuthenticationConfiguration {
    internal readonly string authorizationEndpoint;
    internal readonly string tokenEndpoint;

    /// <summary>
    /// Creates a new authentication configuration object with the given entry points.
    /// </summary>
    /// <param name="authorizationEndpoint">The entry point to navigate end user to for authentication.</param>
    /// <param name="tokenEndpoint">The entry point which is used to exchange the received code to a valid access token.</param>
    public UniWebViewAuthenticationConfiguration(string authorizationEndpoint, string tokenEndpoint)  {
        this.authorizationEndpoint = authorizationEndpoint;
        this.tokenEndpoint = tokenEndpoint;
    }
}

/// <summary>
/// The exception thrown when the authentication flow fails when handling the response.
/// </summary>
public class AuthenticationResponseException : Exception {
    /// <summary>
    /// Exception error code to identify the error type. See the static instance of this class to know detail of error codes.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Creates an authentication response exception.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">A message that contains error detail.</param>
    public AuthenticationResponseException(int code, string message): base(message) {
        Code = code;
    }
    
    /// <summary>
    /// An unexpected authentication callback is received. Error code 7001. 
    /// </summary>
    public static AuthenticationResponseException UnexpectedAuthCallbackUrl 
        = new AuthenticationResponseException(7001, "The received callback url is not expected.");
    
    /// <summary>
    /// The `state` value in the callback url is not the same as the one in the request. Error code 7002.
    /// </summary>
    public static AuthenticationResponseException InvalidState 
        = new AuthenticationResponseException(7002, "The `state` is not valid.");

    /// <summary>
    /// The response is not a valid one. It does not contains a `code` field or cannot be parsed. Error code 7003.
    /// </summary>
    /// <param name="query">The query will be delivered as a part of error message.</param>
    /// <returns>The created response exception that can be thrown out and handled by package user.</returns>
    public static AuthenticationResponseException InvalidResponse(string query) {
        return new AuthenticationResponseException(7003, "The service auth response is not valid: " + query);
    }
}