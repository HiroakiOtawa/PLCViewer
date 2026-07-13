namespace PLCViewer
{
    /// <summary>
    ///  MCプロトコル通信時に発生した異常(PLCからの異常終了コード応答、
    ///  フレーム不正、タイムアウトなど)を表す例外。
    /// </summary>
    public class PlcCommunicationException : Exception
    {
        public PlcCommunicationException(string message)
            : base(message)
        {
        }

        public PlcCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
