namespace ObjectLoader.TypeParsers.Interfaces {
    public interface ITypeParser {
        bool CanParse(string keyword);
        void Parse(string line);
    }
}