using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using FitAndShape;
using System;
using System.Threading;

public class FitAndShapeTest
{
    [UnityTest]
    public IEnumerator LoginTest() => UniTask.ToCoroutine(async () =>
    {
        var cancellationTokenSource = new CancellationTokenSource();

        ILoginModel loginModel = new LoginModel("");

        await loginModel.Login("09012345678", "password", cancellationTokenSource.Token);


    });

    [UnityTest]
    public IEnumerator CsvLoadTest() => UniTask.ToCoroutine(async () =>
    {
        var textAsset = await Addressables.LoadAssetAsync<TextAsset>("Assets/FitAndShape/Csv/measurement_ibody.csv");

        Debug.Log(textAsset.text);


        Csv csv = new Csv();

        csv.SetSourceString(textAsset.text, true);

        Measurement measurement = new Measurement(csv.GetRowValues(0));

        Debug.Log(measurement.ToString());

    });

    [UnityTest]
    public IEnumerator CommentTest() => UniTask.ToCoroutine(async () =>
    {
        string url = "https://api.fit-shape.jp/api/measurements/2201011111/body-distortion-comments?key=0744CE0F-9F0F-43DD-A3F9-6C3F4EF39720";

        var cancellationTokenSource = new CancellationTokenSource();

        ILoadCsvModel loadCsvModel = new LoadCsvModel();

        string csvDate = await loadCsvModel.GetCsvData(url, cancellationTokenSource.Token);

        var array = JsonHelper.FromJson<CommentEntity>(csvDate.Replace("[", "").Replace("]", ""));

        foreach (var item in array)
        {
            item.SetPostureVerifyPoint();

            Debug.Log(item.ToString());
        }
    });

    [UnityTest]
    public void GetPutDataTest()
    {
        try
        {
            BodyDistortionsEntity[] bodyDistortionsEntities= new BodyDistortionsEntity[] { new BodyDistortionsEntity(PostureVerifyPoint.BodyInclinationRatio, PostureCondition.FlatBack) };

            string json = JsonHelper.ToJson(bodyDistortionsEntities);

            Debug.Log(json);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    [UnityTest]
    public IEnumerator PutTest() => UniTask.ToCoroutine(async () =>
    {
        try
        {
            BodyDistortionsEntity[] bodyDistortionsEntities = new BodyDistortionsEntity[] { new BodyDistortionsEntity(PostureVerifyPoint.BodyInclinationRatio, PostureCondition.FlatBack) };

            string json = JsonHelper.ToJson(bodyDistortionsEntities);

            Debug.Log(json);

            //https://api.fit-shape.jp/api/measurements/<measurementNumber>/body-distortions
            //string url = "https://api.fit-shape.jp/api/measurements/2201011111/body-distortions?key=0744CE0F-9F0F-43DD-A3F9-6C3F4EF39720";
            //string url = "https://api.fit-shape.jp/api/measurements/2201011111/body-distortions?key=0744CE0F-9F0F-43DD-A3F9-6C3F4EF39720";
            string url = "https://api.fit-shape.jp/api/measurements/2201011111/body-distortions?key=0744CE0F-9F0F-43DD-A3F9-6C3F4EF39720";

            var cancellationTokenSource = new CancellationTokenSource();

            ISendResultModel sendResultModel = new SendResultModel();

            await sendResultModel.Put(url, json, cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    });

}
