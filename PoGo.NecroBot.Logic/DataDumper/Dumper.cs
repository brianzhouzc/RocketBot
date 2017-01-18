#region using directives

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;
using PoGo.NecroBot.Logic.PoGoUtils;
using PoGo.NecroBot.Logic.State;

#endregion

namespace PoGo.NecroBot.Logic.DataDumper
{
    public static class Dumper
    {
        /// <summary>
        ///     Clears the specified dumpfile.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="filename" />
        /// File to clear/param>
        public static void ClearDumpFile(ISession session, string filename, string extension = "csv")
        {
            var path = Path.Combine(session.LogicSettings.ProfilePath, "Dumps");
            var file = Path.Combine(path,
                $"NecroBot2-{filename}-{DateTime.Today.ToString("yyyy-MM-dd")}-{DateTime.Now.ToString("HH")}.{extension}");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Clears all contents of a file first if overwrite is true
            File.WriteAllText(file, string.Empty);
        }

        /// <summary>
        ///     Dumps data to a file
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data">Dumps the string data to the file</param>
        /// <param name="filename">Filename to be used for naming the file.</param>
        /// <param name="extension">FileExt.</param>
        public static void Dump(ISession session, string[] data, string filename, string extension = "csv")
        {
            string uniqueFileName = $"{filename}";

            DumpToFile(session, data, uniqueFileName, extension);
        }

        /// <summary>
        ///     This is used for dumping contents to a file stored in the Logs folder.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data">Dumps the string data to the file</param>
        /// <param name="filename">Filename to be used for naming the file.</param>
        private static void DumpToFile(ISession session, string[] data, string filename, string extension = "csv")
        {
            var path = Path.Combine(session.LogicSettings.ProfilePath, "Dumps",
                $"NecroBot2-{filename}-{DateTime.Today.ToString("yyyy-MM-dd")}-{DateTime.Now.ToString("HH")}.{extension}");

            CultureInfo culture = CultureInfo.CurrentUICulture;
            string listSeparator = culture.TextInfo.ListSeparator;

            using (var dumpFile = File.AppendText(path))
            {
                string strData = "";
                foreach (string str in data)
                {
                    if (strData != "")
                        strData += listSeparator;

                    if (str.Contains("\""))
                    {
                        strData += string.Format("\"{0}\"", str.Replace("\"", "\"\""));
                    }
                    else if (str.Contains(listSeparator))
                    {
                        strData += string.Format("\"{0}\"", str);
                    }
                    else
                    {
                        strData += str;
                    }
                }
                dumpFile.WriteLine(strData);
                dumpFile.Flush();
            }
        }

        /// <summary>
        ///     Set the dumper.
        /// </summary>
        /// <param name="dumper"></param>
        /// <param name="subPath"></param>
        public static void SetDumper(IDumper dumper, string subPath = "")
        {
        }

        public static async Task SaveAsExcel(ISession session, string Filename = "")
        {
            await Task.Run(() =>
            {
                var allPokemonInBag = session.LogicSettings.PrioritizeIvOverCp
                    ? session.Inventory.GetHighestsPerfect(1000).Result
                    : session.Inventory.GetHighestsCp(1000).Result;
                string file = !string.IsNullOrEmpty(Filename)
                    ? Filename
                    : $"config\\{session.Settings.GoogleUsername}{session.Settings.PtcUsername}\\allpokemon.xlsx";
                int rowNum = 1;
                using (Stream stream = File.OpenWrite(file))
                using (var package = new ExcelPackage(stream))
                {
                    var ws = package.Workbook.Worksheets.Add("Pokemons");
                    foreach (var item in allPokemonInBag)
                    {
                        if (rowNum == 1)
                        {
                            ws.Cells[1, 1].Value = "#";
                            ws.Cells[1, 2].Value = "Pokemon";
                            ws.Cells[1, 3].Value = "Display Name";
                            ws.Cells[1, 4].Value = "Nickname";
                            ws.Cells[1, 5].Value = "IV";
                            ws.Cells[1, 6].Value = "Attack";
                            ws.Cells[1, 7].Value = "Defense";
                            ws.Cells[1, 8].Value = "Stamina";
                            ws.Cells[1, 9].Value = "HP";
                            ws.Cells[1, 10].Value = "MaxHP";
                            ws.Cells[1, 11].Value = "CP";
                            ws.Cells[1, 12].Value = "Candy";
                            ws.Cells[1, 13].Value = "Level";
                            ws.Cells[1, 14].Value = "Move1";
                            ws.Cells[1, 15].Value = "Move2";
                            ws.Cells["A1:O1"].AutoFilter = true;
                            ws.Cells["A1:O1"].Style.Font.Bold = true;
                        }
                        ws.Cells[rowNum + 1, 1].Value = rowNum;
                        ws.Cells[rowNum + 1, 2].Value = item.PokemonId.ToString();
                        ws.Cells[rowNum + 1, 3].Value = item.Nickname;
                        ws.Cells[rowNum + 1, 4].Value = item.OwnerName;
                        ws.Cells[rowNum + 1, 5].Value = Math.Round(PokemonInfo.CalculatePokemonPerfection(item), 2);

                        ws.Cells[rowNum + 1, 6].Value = item.IndividualAttack;
                        ws.Cells[rowNum + 1, 7].Value = item.IndividualDefense;
                        ws.Cells[rowNum + 1, 8].Value = item.IndividualStamina;
                        ws.Cells[rowNum + 1, 9].Value = item.Stamina;
                        ws.Cells[rowNum + 1, 10].Value = item.StaminaMax;
                        ws.Cells[rowNum + 1, 11].Value = PokemonInfo.CalculateCp(item);
                        ws.Cells[rowNum + 1, 12].Value = session.Inventory.GetCandy(item.PokemonId);
                        ws.Cells[rowNum + 1, 13].Value = PokemonInfo.GetLevel(item);
                        ws.Cells[rowNum + 1, 14].Value = item.Move1.ToString();
                        ws.Cells[rowNum + 1, 15].Value = item.Move2.ToString();
                        rowNum++;
                    }
                    package.Save();
                }
            });
        }
    }
}