namespace R3Modeller.CrossPlatform.Controls.Dragger {
    public interface IValueFormatter {
        string ToString(double value, int? roundedPlaces);
    }
}