using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiStore.Services
{
    public class FileIO
    {

        /// <summary>
        /// prompts the user to choose a save location for the CSV backup
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetPath(string name)
        {

            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Choose file name",

                    // TODO: add the playlist name as a default name 
                };
                saveFileDialog.InitialFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
                saveFileDialog.Filters.Add(new FileDialogFilter { Name = "spreadsheets", Extensions = { "csv" } });
                var outPathStrings = await saveFileDialog.ShowAsync(desktop.MainWindow).ConfigureAwait(false);

                //var fileresult = task.Result;
                return String.Join(" ", outPathStrings);
            }

            return "";
        }
    }
}
