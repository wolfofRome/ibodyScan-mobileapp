using System.Linq;

namespace FitAndShape
{
    public interface ICommentModel
    {
        string GetComment(PostureVerifyPoint postureVerifyPoint);
    }

    public sealed class CommentModel : ICommentModel
    {
        readonly CommentEntity[] _commentEntities;

        public CommentModel(string data)
        {
            _commentEntities = JsonHelper.FromJson<CommentEntity>(data.Replace("[", "").Replace("]", ""));

            foreach (CommentEntity item in _commentEntities)
            {
                item.SetPostureVerifyPoint();
            }
        }

        public string GetComment(PostureVerifyPoint postureVerifyPoint)
        {
            if (_commentEntities == null) return string.Empty;

            CommentEntity commentEntity = _commentEntities.Where(n => n.PostureVerifyPoint == postureVerifyPoint).FirstOrDefault();

            if (commentEntity == null) return string.Empty;

            return commentEntity.Comment;
        }
    }
}