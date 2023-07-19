using System.IO;

namespace ObjectLoader.Loaders {
    public abstract class LoaderBase {
        private StreamReader _lineStreamReader;

        protected void StartLoad(Stream lineStream) {
            this._lineStreamReader = new StreamReader(lineStream);

            while (!this._lineStreamReader.EndOfStream) {
                this.ParseLine();
            }
        }

        private void ParseLine() {
            string currentLine = this._lineStreamReader.ReadLine();

            if (string.IsNullOrWhiteSpace(currentLine) || currentLine[0] == '#') {
                return;
            }

            string[] fields = currentLine.Trim().Split(null, 2);
            string keyword = fields[0].Trim();
            string data = fields[1].Trim();

            this.ParseLine(keyword, data);
        }

        protected abstract void ParseLine(string keyword, string data);
    }
}