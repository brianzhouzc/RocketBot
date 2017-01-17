namespace PoGo.NecroBot.Logic.Captcha.Anti_Captcha
{
    public class AnticaptchaTask
    {
        private readonly string _errorCode;
        private readonly string _errorDescription;
        private readonly int? _errorId;
        private readonly int? _taskId;

        public AnticaptchaTask(int? taskId, int? errorId, string errorCode, string errorDescription)
        {
            _errorId = errorId;
            _taskId = taskId;
            _errorCode = errorCode;
            _errorDescription = errorDescription;
        }

        public string GetErrorCode()
        {
            return _errorCode;
        }

        public string GetErrorDescription()
        {
            return _errorDescription;
        }

        public int? GetTaskId()
        {
            return _taskId;
        }

        public int? GetErrorId()
        {
            return _errorId;
        }

        public override string ToString()
        {
            return "AnticaptchaTask{" +
                   "errorId=" + _errorId +
                   ", taskId=" + _taskId +
                   ", errorCode='" + _errorCode + '\'' +
                   ", errorDescription='" + _errorDescription + '\'' +
                   '}';
        }
    }
}