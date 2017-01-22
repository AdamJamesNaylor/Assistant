namespace Imbick.Assistant.Core {
    using System.IO;
    using System.Text;

    public static class StreamExtensions {
        public static byte[] ReadLineAsBytes(this Stream stream) {

            using (var memoryStream = new MemoryStream()) {
                while (true) {
                    int justRead = stream.ReadByte();
                    if (justRead == -1 && memoryStream.Length > 0)
                        break;

                    // Check if we started at the end of the stream we read from
                    // and we have not read anything from it yet
                    if (justRead == -1 && memoryStream.Length == 0)
                        return null;

                    char readChar = (char) justRead;

                    // Do not write \r or \n
                    if (readChar != '\r' && readChar != '\n')
                        memoryStream.WriteByte((byte) justRead);

                    // Last point in CRLF pair
                    if (readChar == '\n')
                        break;
                }

                return memoryStream.ToArray();
            }
        }

        public static string ReadLineAsAscii(this Stream stream) {
            byte[] readFromStream = ReadLineAsBytes(stream);
            return readFromStream != null ? System.Text.Encoding.ASCII.GetString(readFromStream) : null;
        }


    }
}