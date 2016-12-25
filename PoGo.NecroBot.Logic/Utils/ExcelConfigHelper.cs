using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PoGo.NecroBot.Logic.Utils
{
    public class ExcelConfigHelper
    {
        private static int OFFSET_START = 4;
        private static int COL_OFFSET = 9;

        public static void MigrateFromObject(GlobalSettings setting, string destination)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PoGo.NecroBot.Logic.config.xlsm";

            BackwardCompitableUpdate(setting);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var package = new ExcelPackage(stream))
            {
                var pokemonFilter = package.Workbook.Worksheets["Pokemons"];
                pokemonFilter.Protection.IsProtected = true;
                pokemonFilter.Cells["J2:AZ155"].Style.Locked = false;
                MigrateItemRecycleFilter(package, setting);

                foreach (var item in setting.GetType().GetFields())
                {
                    var att = item.GetCustomAttributes<ExcelConfigAttribute>(true).FirstOrDefault();
                    if (att != null)
                    {
                        ExcelWorksheet workSheet = BuildSheetHeader(package, item, att);

                        var configProp = item.GetValue(setting);
                        if (item.FieldType.IsGenericType && item.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            var type = configProp.GetType();
                            Type keyType = type.GetGenericArguments()[0];
                            Type valueType = type.GetGenericArguments()[1];

                            MethodInfo method = typeof(ExcelConfigHelper).GetMethod("BuildDictionaryDataSheet", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                            MethodInfo genericMethod = method.MakeGenericMethod(valueType);
                            genericMethod.Invoke(null, new object[] { workSheet, configProp });

                        }
                        else
                        {
                            BuildCustomObjectData(workSheet, configProp);
                        }

                        workSheet.Protection.IsProtected = true;
                    }

                }
                package.SaveAs(new FileInfo(destination));
            }
        }

        private static void BackwardCompitableUpdate(GlobalSettings setting)
        {      
            foreach (var item in setting.PokemonsTransferFilter)
            {
                item.Value.AllowTransfer = true;
            }
            foreach (var item in setting.PokemonsNotToTransfer)
            {
                if (setting.PokemonsTransferFilter.ContainsKey(item))
                {
                    setting.PokemonsTransferFilter[item].DoNotTransfer = true;
                }
                else
                {
                    setting.PokemonsTransferFilter.Add(item, new TransferFilter()
                    {
                        AllowTransfer = true,
                        DoNotTransfer = true
                    });
                }
            }
            foreach (var item in setting.PokemonsToEvolve)
            {
                if(!setting.EvolvePokemonFilter.ContainsKey(item))
                {
                    setting.EvolvePokemonFilter.Add(item, new EvolveFilter()
                    {
                       
                    });
                }
            }
        }

        private static void BuildDictionaryDataSheet<T>(ExcelWorksheet sheet, Dictionary<PokemonId, T> dictionary)
        {
            //Dictionary<PokemonId, object> dictionary = input;
            var type = dictionary.GetType();
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];
            int col = 0;
            for (int i = 1; i <= 151; i++)
            {
                int id = sheet.Cells[4 + i, 1].GetValue<int>();
                var pokemonId = (PokemonId)id;
                if (dictionary.ContainsKey(pokemonId))
                {
                    var obj = dictionary[pokemonId];

                    foreach (var prop in valueType.GetProperties())
                    {
                        var att = prop.GetCustomAttribute<ExcelConfigAttribute>();
                        if (att != null)
                        {
                            col = Math.Max(col, att.Position);
                            if(att.IsPrimaryKey)
                            {
                                sheet.Cells[4 + i, COL_OFFSET + att.Position].Value = true;
                                continue;
                            }
                            var val = prop.GetValue(obj);

                            if (prop.PropertyType == typeof(List<List<PokemonMove>>) && val != null)
                            {
                                sheet.Cells[4 + i, COL_OFFSET + att.Position].Value = "[]";
                            }
                            else
                            {
                                sheet.Cells[4 + i, COL_OFFSET + att.Position].Value = val;

                            }
                        }
                        else
                        {
                           //maybe throw exception, 
                        }
                    }
                }


            }
            //sheet.Cells[$"A1:{GetCol(col)}:253"].AutoFilter = true;
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            for (int i = 3; i <= COL_OFFSET; i++)
            {
                sheet.Column(i).Hidden = true;
            }
            sheet.Protection.AllowAutoFilter = true;
            sheet.Protection.AllowDeleteRows = false;
            sheet.Protection.AllowInsertColumns = false;
        }

        private static void BuildCustomObjectData(ExcelWorksheet workSheet, object configProp)
        {
            foreach (var cfg in configProp.GetType().GetFields())
            {
                WriteOnePropertyToSheet(workSheet, configProp, cfg);
            }
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
        }

        private static void WriteOnePropertyToSheet(ExcelWorksheet workSheet, object configProp, FieldInfo cfg)
        {
            var att2 = cfg.GetCustomAttributes(typeof(ExcelConfigAttribute), true).FirstOrDefault();
            if (att2 != null)
            {
                var exAtt = att2 as ExcelConfigAttribute;
                string configKey = string.IsNullOrEmpty(exAtt.Key) ? cfg.Name : exAtt.Key;
                var propValue = cfg.GetValue(configProp);
                workSheet.Cells[exAtt.Position + OFFSET_START, 1].Value = configKey;
                workSheet.Cells[exAtt.Position + OFFSET_START, 2].Value = propValue;
                workSheet.Cells[exAtt.Position + OFFSET_START, 2].Style.Locked = false;
                workSheet.Cells[exAtt.Position + OFFSET_START, 2].Style.Font.Bold = true;
                workSheet.Cells[exAtt.Position + OFFSET_START, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells[exAtt.Position + OFFSET_START, 3].Value = exAtt.Description;
                workSheet.Cells[exAtt.Position + OFFSET_START, 3].Style.Locked = false;
                workSheet.Cells[exAtt.Position + OFFSET_START, 1].AutoFitColumns();
                workSheet.Cells[exAtt.Position + OFFSET_START, 2].AutoFitColumns();
                workSheet.Cells[exAtt.Position + OFFSET_START, 3].AutoFitColumns();
                //AddValidationForType(workSheet, cfg, $"B{exAtt.Position + OFFSET_START}");
                if (propValue is Boolean)
                {
                    var validation = workSheet.DataValidations.AddListValidation($"B{exAtt.Position + OFFSET_START}");
                    validation.ShowErrorMessage = true;
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    validation.Error = "Please select from list";
                    validation.ErrorTitle = $"{configKey} Validation";
                    validation.Formula.Values.Add("TRUE");
                    validation.Formula.Values.Add("FALSE");
                    validation.PromptTitle = "Boolean only";
                    validation.Prompt = "Only TRUE or FALSE are accepted";
                    validation.ShowInputMessage = true;
                    //data validation
                }

                if (propValue is int || propValue is double)
                {
                    var validation = workSheet.DataValidations.AddIntegerValidation($"B{exAtt.Position + OFFSET_START}");
                    validation.ShowErrorMessage = true;
                    validation.Error = "Please enter a valid number";
                    validation.ErrorTitle = $"{configKey} Validation";
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    validation.PromptTitle = "Enter a integer value here";
                    validation.Prompt = "Please enter a negative number here";
                    validation.ShowInputMessage = true;
                    validation.ShowErrorMessage = true;
                    validation.Operator = ExcelDataValidationOperator.between;
                    validation.Formula.Value = 0;
                    validation.Formula2.Value = int.MaxValue;
                    var range = cfg.GetCustomAttributes(typeof(RangeAttribute), true).Cast<RangeAttribute>().FirstOrDefault();
                    if (range != null)
                    {
                        validation.Formula.Value = (int)range.Minimum;
                        validation.Formula2.Value = (int)range.Maximum;
                        validation.Prompt = $"Please enter a valid number from {validation.Formula.Value} to {validation.Formula2.Value}";
                        validation.Error = $"Please enter a valid number from {validation.Formula.Value} to {validation.Formula2.Value}";
                    }
                }
                if (propValue is string)
                {
                    var maxLength = cfg.GetCustomAttributes(typeof(MaxLengthAttribute), true).Cast<MaxLengthAttribute>().FirstOrDefault();
                    var minLength = cfg.GetCustomAttributes(typeof(MinLengthAttribute), true).Cast<MinLengthAttribute>().FirstOrDefault();
                    if (maxLength != null && minLength != null)
                    {
                        var validation = workSheet.DataValidations.AddTextLengthValidation($"B{exAtt.Position + OFFSET_START}");
                        validation.ErrorTitle = $"{configKey} Validation";
                        validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        validation.PromptTitle = "String Validation";
                        validation.ShowInputMessage = true;
                        validation.ShowErrorMessage = true;

                        validation.Error = $"Please enter a string from {minLength.Length} to {maxLength.Length} characters";
                        validation.Prompt = $"Please enter a string from {minLength.Length} to {maxLength.Length} characters";

                        validation.Operator = ExcelDataValidationOperator.between;
                        validation.Formula.Value = minLength.Length;
                        validation.Formula2.Value = maxLength.Length;
                    }
                    else
                    {
                        if (minLength != null)
                        {
                            var validation = workSheet.DataValidations.AddTextLengthValidation($"B{exAtt.Position + OFFSET_START}");
                            validation.ErrorTitle = $"{configKey} Validation";
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                            validation.PromptTitle = "String Validation";
                            validation.ShowInputMessage = true;
                            validation.ShowErrorMessage = true;

                            validation.Error = $"Please enter a string atleast {minLength.Length} characters";
                            validation.Prompt = $"Please enter a string atleast {minLength.Length} characters";

                            validation.Operator = ExcelDataValidationOperator.greaterThan;
                            validation.Formula.Value = minLength.Length;
                        }
                    }

                }
                var enumDataType = cfg.GetCustomAttributes(typeof(EnumDataTypeAttribute), true).Cast<EnumDataTypeAttribute>().FirstOrDefault();
                if (enumDataType != null)
                {
                    var validation = workSheet.DataValidations.AddListValidation($"B{exAtt.Position + OFFSET_START}");
                    validation.ErrorTitle = $"{configKey} Validation";
                    validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                    validation.PromptTitle = $"{configKey} Validation";
                    validation.ShowInputMessage = true;
                    validation.ShowErrorMessage = true;

                    List<string> values = new List<string>();
                    foreach (var v in Enum.GetValues(enumDataType.EnumType))
                    {
                        validation.Formula.Values.Add(v.ToString());
                        values.Add(v.ToString());
                    }
                    string value = String.Join(",", values);
                    validation.Error = $"Please select data from a list: {value}";
                    validation.Prompt = $"Please select data from a list: {value}";
                }
            }
        }

        public static string GetCol(int col)
        {
            return Convert.ToChar(col + 64).ToString();
        }
        private static ExcelWorksheet BuildSheetHeader(ExcelPackage package, FieldInfo item, object att)
        {
            ExcelConfigAttribute excelAtt = att as ExcelConfigAttribute;
            ExcelWorksheet workSheet = package.Workbook.Worksheets[excelAtt.SheetName];
            if (workSheet == null)
            {
                var type = item.FieldType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var pkmRef = package.Workbook.Worksheets["Pokemons"];

                    workSheet = package.Workbook.Worksheets.Add(excelAtt.SheetName, pkmRef);
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];
                    int pos = 1;

                    workSheet.Cells[1, 1].Value = excelAtt.SheetName;
                    workSheet.Cells[2, 1].Value = excelAtt.Description;

                    foreach (var vtp in valueType.GetProperties())
                    {
                        var att1 = vtp.GetCustomAttributes<ExcelConfigAttribute>(true).FirstOrDefault();
                        int colIndex = (att1 == null ? pos : att1.Position) + COL_OFFSET;
                        workSheet.Column(colIndex).AutoFit();
                        workSheet.Cells[4, colIndex].Value = att1 == null ? vtp.Name : att1.Key;
                        if (att1 != null)
                        {
                            workSheet.Cells[4, colIndex].AddComment(att1.Description, "necrobot2");
                            AddValidationForType(workSheet, vtp, $"{GetCol(colIndex)}5:{GetCol(colIndex)}155");
                        }
                        pos++;
                    }
                    workSheet.Cells[$"A1:{GetCol(COL_OFFSET + pos)}1"].Merge = true;
                    workSheet.Cells[$"A2:{GetCol(COL_OFFSET + pos)}2"].Merge = true;
                    workSheet.Cells[$"A1:{GetCol(COL_OFFSET + pos)}1"].Style.Font.Size = 16;
                }
                else {
                    workSheet = package.Workbook.Worksheets.Add(excelAtt.SheetName);
                    workSheet.Cells[1, 1].Value = excelAtt.SheetName;
                    workSheet.Cells[2, 1].Value = excelAtt.Description;

                    workSheet.Cells[$"A1:C1"].Merge = true; ;
                    workSheet.Cells[$"A2:C2"].Merge = true; ;

                    workSheet.Cells["A1:C1"].Style.Font.Size = 16;
                    workSheet.Row(1).CustomHeight = true;
                    workSheet.Row(1).Height = 30;

                    workSheet.Cells["A1:C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells["A1:C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Justify;

                    workSheet.Cells[4, 1].Value = "Key";
                    workSheet.Cells[4, 2].Value = "Value";
                    workSheet.Cells[4, 3].Value = "Description";
                }

                workSheet.Row(4).Style.Font.Bold = true;
            }

            return workSheet;
        }

        private static void AddValidationForType(ExcelWorksheet sheet, PropertyInfo vtp, string address)
        {
            var type = vtp.PropertyType;
            if (type == typeof(bool))
            {
                AddListValidation(sheet, address, $"{type.Name}- Validation", "TRUE or FALSE only", "TRUE", "FALSE");
            }
            if (type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(long))
            {
                var range = vtp.GetCustomAttribute<RangeAttribute>();
                if (range != null)
                {
                    AddNumberValidation(sheet, address, $"{ type.Name} - Validation", $"Any number from {range.Minimum} to {range.Maximum}", (int)range.Minimum, (int)range.Maximum);
                }
                else
                { }
            }

            var enumtype = vtp.GetCustomAttribute<EnumDataTypeAttribute>();
            if (enumtype != null)
            {
                AddEnumValidation(sheet, address, $"{type.Name}- Validation", "Select item from the list", enumtype);
            }
        }


        public static void MigrateItemRecycleFilter(ExcelPackage package, GlobalSettings setting)
        {
            var workSheet = package.Workbook.Worksheets.Add("ItemRecycleFilter");

            workSheet.Cells[1, 1].Value = "ItemRecycleFilter";
            workSheet.Cells[2, 1].Value = "Special number of each item to keep ";

            workSheet.Cells["A1:C1"].Merge = true; ;
            workSheet.Cells["A1:C1"].Style.Font.Size = 16;
            workSheet.Row(1).CustomHeight = true;
            workSheet.Row(1).Height = 30;

            workSheet.Cells["A1:C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1:C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Justify;

            workSheet.Cells[4, 1].Value = "Item Type";
            workSheet.Cells[4, 2].Value = "Value";
            workSheet.Row(4).Style.Font.Bold = true;
            int index = 1;
            foreach (var item in setting.ItemRecycleFilter)
            {
                workSheet.Cells[index + OFFSET_START, 1].Value = item.Key.ToString();

                workSheet.Cells[index + OFFSET_START, 2].Value = item.Value;
                workSheet.Cells[index + OFFSET_START, 2].Style.Font.Bold = true;
                workSheet.Cells[index + OFFSET_START, 2].Style.Locked = false;
                AddNumberValidation(workSheet, $"B{index + OFFSET_START}", "Item filter validation", "Number from 0 to 10000", 0, 10000);
                index++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Protection.IsProtected = true;
        }

        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
        public static GlobalSettings ReadExcel(GlobalSettings setting, string configFile)
        {
            if(File.Exists(configFile +".tmp"))
            {
                //need rename the config.xlsm by the .tmp file 
                try
                {
                    File.Delete(configFile);//remove existing config file
                    File.Move(configFile + ".tmp", configFile);
                    //File.Delete(configFile + ".tmp");
                }
                catch (Exception )
                {
                    Logger.Write("Seem that you are opening config.xlsm, You need to close it for migration new config.");
                }
            }
            bool needSave = false;
            using (FileStream stream = File.Open(configFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(stream))
            {
                foreach (var item in setting.GetType().GetFields())
                {
                    var att = item.GetCustomAttributes(typeof(ExcelConfigAttribute), true).Cast<ExcelConfigAttribute>().FirstOrDefault();
                    if (att != null)
                    {
                        var ws = package.Workbook.Worksheets[att.SheetName];
                        var configProp = item.GetValue(setting);

                        if (item.FieldType.IsGenericType && item.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            var type = item.FieldType;
                            Type keyType = type.GetGenericArguments()[0];
                            Type valueType = type.GetGenericArguments()[1];

                            MethodInfo method = typeof(ExcelConfigHelper).GetMethod("ReadSheetAsDictionary", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                            MethodInfo genericMethod = method.MakeGenericMethod(valueType);
                            configProp = genericMethod.Invoke(null, new object[] { ws });

                            //configProp = ReadSheetAsDictionary<TransferFilter>(ws);
                        }
                        else {
                            foreach (var cfg in configProp.GetType().GetFields())
                            {
                                var peAtt = cfg.GetCustomAttributes(typeof(ExcelConfigAttribute), true).Cast<ExcelConfigAttribute>().FirstOrDefault();
                                if (peAtt != null)
                                {
                                    string key = string.IsNullOrEmpty(peAtt.Key) ? cfg.Name : peAtt.Key;
                                    string keyFromExcel = ws.Cells[$"A{peAtt.Position + OFFSET_START}"].GetValue<string>();
                                    if (keyFromExcel == key)
                                    {
                                        var value = ws.Cells[$"B{peAtt.Position + OFFSET_START}"].Value;
                                        var convertedValue = System.Convert.ChangeType(value, cfg.FieldType);
                                        cfg.SetValue(configProp, convertedValue);
                                    }
                                    else
                                    {    //migrate config
                                        needSave = true;
                                        WriteOnePropertyToSheet(ws, configProp, cfg);
                                    }
                                }
                            }
                        }
                        item.SetValue(setting, configProp);
                        //set to original object
                    }
                }
                var pkmSheet = package.Workbook.Worksheets["Pokemons"];
                setting.ItemRecycleFilter = ReadItemRecycleFilter(package);
                if (needSave || hasUpdate)
                {
                    package.SaveAs(new FileInfo(configFile +".tmp"));  //use to migrate new config, hack hack hack
                }
            }
           
            return ConvertToBackwardCompitable(setting);
        }

        private static GlobalSettings ConvertToBackwardCompitable(GlobalSettings setting)
        {
            if (setting.PokemonsTransferFilter != null)
            {
                setting.PokemonsNotToTransfer = setting.PokemonsTransferFilter.Where(p => p.Value.DoNotTransfer).Select(p => p.Key).ToList();
            }
            setting.PokemonsToEvolve = setting.EvolvePokemonFilter.Select(x => x.Key).ToList();

            if(setting.SnipePokemonFilter!= null)
            {
                setting.PokemonToSnipe.Pokemon = setting.SnipePokemonFilter.Select(p => p.Key).ToList();
            }
            return setting;

        }

        private static bool hasUpdate;
        private static Dictionary<PokemonId, T> ReadSheetAsDictionary<T>(ExcelWorksheet ws)
        {
            hasUpdate = hasUpdate || SyncHeader<T>(ws);
            Dictionary<PokemonId, T> results = new Dictionary<PokemonId, T>();
            for (int i = 5; i <= 155; i++)
            {
                T obj = Activator.CreateInstance<T>();

                var id = ws.Cells[i, 1].GetValue<int>();
                var pokemonId = (PokemonId)id;

                foreach (var prop in typeof(T).GetProperties())
                {
                    var attr = prop.GetCustomAttribute<ExcelConfigAttribute>();
                    if (attr != null)
                    {
                        var celvalue = ws.Cells[i, COL_OFFSET + attr.Position].Value;
                        if (celvalue == null && attr.IsPrimaryKey) continue;
                        
                        if (celvalue != null)
                        {
                            if (prop.PropertyType == typeof(List<List<PokemonMove>>) && celvalue != null)
                            {
                                prop.SetValue(obj, ParseMoves(celvalue.ToString()));
                            }
                            else {
                                var convertedVal = Convert.ChangeType(celvalue, prop.PropertyType);
                                prop.SetValue(obj, convertedVal);
                                if (attr.IsPrimaryKey && (bool)convertedVal)
                                {
                                    results.Add(pokemonId, obj);
                                }
                            }
                        }
                        else
                        {
                                ws.Cells[i, COL_OFFSET + attr.Position].Value = prop.GetValue(obj);
                        }

                    }
                }
            }
            return results;
        }

        private static bool SyncHeader<T>(ExcelWorksheet ws)
        {
            bool needUpdate = false;
            var type = typeof(T);
            foreach (var fi in type.GetProperties())
            {
                var attr = fi.GetCustomAttributes<ExcelConfigAttribute>(true).FirstOrDefault();
                if (attr != null)
                {
                    var cell = ws.Cells[4, attr.Position + COL_OFFSET];

                    var cellHeader = ws.Cells[4, attr.Position + COL_OFFSET].Value;
                    if (cellHeader != null) {
                        cell.Value = attr.Key;
                    }
                    else
                    {
                        needUpdate = true;
                        cell.Value = attr.Key;
                        AddValidationForType(ws, fi, $"{GetCol(attr.Position + COL_OFFSET)}5:{GetCol(attr.Position + COL_OFFSET)}155");
                    }
                }
            }
            return needUpdate;
        }

        private static List<ItemRecycleFilter> ReadItemRecycleFilter(ExcelPackage package)
        {
            List<ItemRecycleFilter> result = new List<ItemRecycleFilter>();
            var worksheet = package.Workbook.Worksheets["ItemRecycleFilter"];
            string key = "";
            int row = OFFSET_START + 1;
            do
            {
                key = worksheet.Cells[row, 1].GetValue<String>();

                if (!string.IsNullOrEmpty(key))
                {
                    int val = worksheet.Cells[row, 2].GetValue<int>();
                    result.Add(new ItemRecycleFilter()
                    {
                        Key = (ItemId)Enum.Parse(typeof(ItemId), key),
                        Value = val
                    });
                }
                row++;

            }
            while (!string.IsNullOrEmpty(key));
            return result;
        }

        public static Dictionary<PokemonId, T> ReadListObjectAsDictionary<T>(ExcelWorksheet sheet, string column, bool compare)
        {
            Dictionary<PokemonId, T> results = new Dictionary<PokemonId, T>();
            for (int i = 4; i <= 155; i++)
            {
                string address = $"{column}{i}";
                var isAllow = Convert.ToBoolean(sheet.Cells[address].GetValue<string>());
                if (isAllow == compare)
                {
                    int id = sheet.Cells[$"A{i}"].GetValue<int>();

                    var pokemonId = (PokemonId)id;
                    var obj = (T)Activator.CreateInstance(typeof(T));

                    foreach (var fi in typeof(T).GetProperties())
                    {
                        var attr = fi.GetCustomAttributes<ExcelConfigAttribute>(true).FirstOrDefault();
                        if (attr != null)
                        {
                            string addr = $"{attr.Key}{i}";
                            var v = sheet.Cells[addr].Value;
                            if (fi.PropertyType == typeof(List<List<PokemonMove>>) && v != null)
                            {
                                fi.SetValue(obj, ParseMoves(v.ToString()));
                            }
                            else {
                                if (v == null)
                                {
                                    //throw exception

                                }
                                var converted = Convert.ChangeType(v, fi.PropertyType);
                                fi.SetValue(obj, converted);
                            }
                        }
                    }

                    foreach (var fi in typeof(T).GetFields())
                    {
                        var attr = fi.GetCustomAttributes<ExcelConfigAttribute>(true).FirstOrDefault();
                        if (attr != null)
                        {
                            string addr = $"{attr.Key}{i}";
                            var v = sheet.Cells[addr].Value;
                            if (fi.FieldType == typeof(List<List<PokemonMove>>))
                            {
                                fi.SetValue(obj, ParseMoves(v.ToString()));
                            }
                            else {
                                var converted = Convert.ChangeType(v, fi.FieldType);
                                fi.SetValue(obj, converted);
                            }
                        }
                    }

                    results.Add(pokemonId, obj);
                }
            }
            return results;
        }

        public static List<List<PokemonMove>> ParseMoves(string moves)
        {
            List<List<PokemonMove>> results = new List<List<PokemonMove>>();

            string regexPattern = @"\[([a-zA-Z\s]*),([a-zA-Z\s]*)\]";
            var matches = Regex.Matches(moves, regexPattern);
            foreach (Match match in matches)
            {
                try
                {
                    string move1 = match.Groups[1].Value;
                    string move2 = match.Groups[2].Value;
                    PokemonMove pmove1 = (PokemonMove)Enum.Parse(typeof(PokemonMove), move1.Replace(" ", ""));
                    PokemonMove pmove2 = (PokemonMove)Enum.Parse(typeof(PokemonMove), move2.Replace(" ", ""));
                    results.Add(new List<PokemonMove>() {
                    pmove1, pmove2
                });
                }
                catch (Exception) { }

            }
            return results;
        }
      
        public static void AddListValidation(ExcelWorksheet pokemonFilter, string address, string errorTitle, string promptTitle, params string[] values)
        {
            var validation = pokemonFilter.DataValidations.AddListValidation(address);
            validation.ShowErrorMessage = true;
            validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            validation.Error = "Please select from list";
            validation.ErrorTitle = errorTitle;
            validation.AllowBlank = false;
            foreach (var item in values)
            {
                validation.Formula.Values.Add(item);
            }

            validation.PromptTitle = promptTitle;
            validation.Prompt = $"ONLY {string.Join(",", values) } are accepted";
            validation.ShowInputMessage = true;
        }
        public static void AddEnumValidation(ExcelWorksheet workSheet, string address, string errorTitle, string promptTitle, EnumDataTypeAttribute enumDataType)
        {
            //var enumDataType = cfg.GetCustomAttributes(typeof(EnumDataTypeAttribute), true).Cast<EnumDataTypeAttribute>().FirstOrDefault();
            if (enumDataType != null)
            {
                var validation = workSheet.DataValidations.AddListValidation(address);
                validation.ErrorTitle = $"Enum Validation";
                validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                validation.PromptTitle = $"Select item from the list";
                validation.ShowInputMessage = true;
                validation.ShowErrorMessage = true;

                List<string> values = new List<string>();
                foreach (var v in Enum.GetValues(enumDataType.EnumType))
                {
                    validation.Formula.Values.Add(v.ToString());
                    values.Add(v.ToString());
                }
                string value = String.Join(",", values);
                validation.Error = $"Please select data from a list: {value}";
                validation.Prompt = $"Please select data from a list: {value}";
            }
        }

        public static void AddNumberValidation(ExcelWorksheet workSheet, string address, string errorTitle, string promptTitle, int? minValue, int? maxValue)
        {
            var validation = workSheet.DataValidations.AddIntegerValidation(address);
            validation.ShowErrorMessage = true;
            validation.Error = "Please enter a valid number";
            validation.ErrorTitle = errorTitle;
            validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
            validation.PromptTitle = promptTitle;
            validation.Prompt = "Please enter a negative number here";
            validation.ShowInputMessage = true;
            validation.ShowErrorMessage = true;
            validation.Operator = ExcelDataValidationOperator.between;
            validation.Formula.Value = 0;
            validation.Formula2.Value = int.MaxValue;
            validation.AllowBlank = false;
            if (minValue.HasValue)
            {
                validation.Formula.Value = (int)minValue.Value;
            }
            if (maxValue.HasValue)
            {
                validation.Formula2.Value = (int)maxValue.Value;

            }

            if (maxValue.HasValue || minValue.HasValue)

            {
                validation.Prompt = $"Please enter a valid number from {validation.Formula.Value} to {validation.Formula2.Value}";
                validation.Error = $"Please enter a valid number from {validation.Formula.Value} to {validation.Formula2.Value}";
            }

        }
    }
}
