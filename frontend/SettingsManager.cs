using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace BwetForecasterWinForms
{
    public static class SettingsManager
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BwetForecaster",
            "settings.json");

        public static string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static Rectangle? LoadWindowBounds()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return null;

                var settings = JsonConvert.DeserializeObject<WindowSettings>(File.ReadAllText(FilePath));
                if (settings == null || settings.Width < 400 || settings.Height < 300)
                    return null;

                var bounds = new Rectangle(settings.X, settings.Y, settings.Width, settings.Height);
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    if (screen.WorkingArea.IntersectsWith(bounds))
                        return bounds;
                }
            }
            catch
            {
            }

            return null;
        }

        public static void SaveWindowBounds(Rectangle bounds)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(new WindowSettings
                {
                    X = bounds.X,
                    Y = bounds.Y,
                    Width = bounds.Width,
                    Height = bounds.Height
                }));
            }
            catch
            {
            }
        }

        private sealed class WindowSettings
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
