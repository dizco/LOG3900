using System.Windows.Forms;

namespace PolyPaint.Helpers
{
    internal static class UserAlerts
    {
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, @"Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
