using System;

namespace R3Modeller.Utils {
    public static class Validate {
        public static void ValidateNotNull(object value, string param, string msg) {
            if (value == null) {
                throw new ArgumentNullException(param, msg);
            }
        }
    }
}