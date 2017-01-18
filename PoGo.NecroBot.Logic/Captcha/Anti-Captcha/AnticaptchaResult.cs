namespace PoGo.NecroBot.Logic.Captcha.Anti_Captcha
{
    public class AnticaptchaResult
    {
        public enum Status
        {
            ready,
            unknown,
            processing
        }

        private readonly double? _cost;
        private readonly int? _createTime;
        private readonly int? _endTime;
        private readonly string _errorCode;
        private readonly string _errorDescription;
        private readonly int? _errorId;
        private readonly string _ip;
        private readonly string _solution;
        private readonly int? _solveCount;
        private readonly Status? _status;

        public AnticaptchaResult(Status? status, string solution, int? errorId, string errorCode,
            string errorDescription,
            double? cost, string ip, int? createTime, int? endTime, int? solveCount)
        {
            _errorId = errorId;
            _errorCode = errorCode;
            _errorDescription = errorDescription;
            _status = status;
            _solution = solution;
            _cost = cost;
            _ip = ip;
            _createTime = createTime;
            _endTime = endTime;
            _solveCount = solveCount;
        }

        public override string ToString()
        {
            return "AnticaptchaResult{" +
                   "errorId=" + _errorId +
                   ", errorCode='" + _errorCode + '\'' +
                   ", errorDescription='" + _errorDescription + '\'' +
                   ", status=" + _status +
                   ", solution='" + _solution + '\'' +
                   ", cost=" + _cost +
                   ", ip='" + _ip + '\'' +
                   ", createTime=" + _createTime +
                   ", endTime=" + _endTime +
                   ", solveCount=" + _solveCount +
                   '}';
        }

        public int? GetErrorId()
        {
            return _errorId;
        }

        public string GetErrorCode()
        {
            return _errorCode;
        }

        public string GetErrorDescription()
        {
            return _errorDescription;
        }

        public Status? GetStatus()
        {
            return _status;
        }

        public string GetSolution()
        {
            return _solution;
        }

        public double? GetCost()
        {
            return _cost;
        }

        public string GetIp()
        {
            return _ip;
        }

        public int? GetCreateTime()
        {
            return _createTime;
        }

        public int? GetEndTime()
        {
            return _endTime;
        }

        public int? GetSolveCount()
        {
            return _solveCount;
        }
    }
}