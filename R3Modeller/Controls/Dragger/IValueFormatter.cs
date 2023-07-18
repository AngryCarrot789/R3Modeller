namespace R3Modeller.Controls.Dragger {
    public interface IValueFormatter {
        string ToString(double value, int? roundedPlaces);
    }
}