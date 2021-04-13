using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ImageShaper;
using System.Drawing;

public static class Cinimanager
{
    static IniCache _inis = new IniCache();

    public struct TSETTINGS
    {
        public SHP_TS_EncodingFormat DefaultCompression;
        public Point StartPosition;
        public Size StartSize;

        public bool ShowPreview;
        public string LastPalette;

        public bool PreventTSWobbleBug;
        public bool CreateImages;
        public string CreateImages_FileName;
        public string CreateImages_Format;

        public bool OptimizeCanvas;
        public bool KeepCentered;

        public Color RadarColor;
        public bool AverageRadarColor;

        public bool UseCustomBackgroundColor;
        public Color CustomBackgroundColor;
        public bool CombineTransparentPixel;

        public string OutputFolder;
        public string PreviewBackgroundImage;

        public string LastFireFLHFinderDirectory;
    }

    public static string inifilename = "ImageShaper.ini";

    public static TSETTINGS inisettings;

    private static void IniWriteValue(string Section, string Key, string Value)
    {
        var ini = _inis.GetOrOpenOrCreate(inifilename);
        ini.GetOrAddSection(Section).Add(Key, Value);
        _inis.Save(inifilename);
    }

    private static string GetIniFileString(string inifilename, string category, string key, string defaultValue)
    {
        var section = _inis.GetOrOpenOrCreate(inifilename).GetSection(category);
        return section.GetValue(key, defaultValue);
    }

    private static List<string> GetSections(string inifilename)
    {
        return _inis.GetOrOpenOrCreate(inifilename).SectionNames.ToList();
    }

    private static void RemoveIniSection(string sectionname, string filename)
    {
        _inis.GetOrOpenOrCreate(filename).RemoveSection(sectionname);
    }

    private static List<string> GetKeys(string inifilename, string category)
    {
        return _inis.GetOrOpenOrCreate(inifilename).GetSection(category).Keys.ToList();
    }

    public static int ClearIniFile(string filename)
    {
        if (System.IO.File.Exists(filename))
        {
            _inis.Close(filename);
            System.IO.File.Delete(filename);
            return 0;
        }
        else return 1;
    }

    public static int SaveIniSettings()
    {
        var section = _inis.GetOrOpenOrCreate(inifilename).GetOrAddSection("General");
        section.Add("X_StartPosition", inisettings.StartPosition.X.ToString());
        section.Add("Y_StartPosition", inisettings.StartPosition.Y.ToString());
        section.Add("StartWidth", inisettings.StartSize.Width.ToString());
        section.Add("StartHeight", inisettings.StartSize.Height.ToString());
        section.Add("DefaultCompression", inisettings.DefaultCompression.ToString());
        section.Add("ShowPreview", inisettings.ShowPreview.ToString());
        section.Add("LastPalette", inisettings.LastPalette);

        section.Add("CreateImages", inisettings.CreateImages.ToString());
        section.Add("CreateImages_FileName", inisettings.CreateImages_FileName);
        section.Add("CreateImages_Format", inisettings.CreateImages_Format);
        section.Add("PreventTSWobbleBug", inisettings.PreventTSWobbleBug.ToString());

        section.Add("OptimizeCanvas", inisettings.OptimizeCanvas.ToString());
        section.Add("KeepCentered", inisettings.KeepCentered.ToString());

        section.Add("RadarColor", ColorToStr(inisettings.RadarColor, true));
        section.Add("AverageRadarColor", inisettings.AverageRadarColor.ToString());

        section.Add("UseCustomBackgroundColor", inisettings.UseCustomBackgroundColor.ToString());
        section.Add("CustomBackgroundColor", ColorToStr(inisettings.CustomBackgroundColor, true));
        section.Add("CombineTransparentPixel", inisettings.CombineTransparentPixel.ToString());

        section.Add("OutputFolder", inisettings.OutputFolder);
        section.Add("PreviewBackgroundImage", inisettings.PreviewBackgroundImage);

        section.Add("LastFireFLHFinderDirectory", inisettings.LastFireFLHFinderDirectory);
        
        _inis.Save(inifilename);
        
        if (System.IO.File.Exists(inifilename))
        {
            if ((System.IO.File.GetAttributes(inifilename) & System.IO.FileAttributes.ReadOnly) != System.IO.FileAttributes.ReadOnly)
            {
                return 0;//no problems
            }
            else return 2;//write protected
        }
        else return 1;//file exists
    }

    public static void LoadIniSettings()
    {
        inisettings = new TSETTINGS();
        inisettings.LastPalette = "";
        inisettings.ShowPreview = true;
        inisettings.DefaultCompression = SHP_TS_EncodingFormat.Detect_best_size;

        string returnString = new string(' ', 1024);

        int X = 0, Y = 0;
        int.TryParse(GetIniFileString(inifilename, "General", "X_StartPosition", "0"), out X);
        int.TryParse(GetIniFileString(inifilename, "General", "Y_StartPosition", "0"), out Y);
        inisettings.StartPosition = new Point(X, Y);
        int W = 0, H = 0;
        int.TryParse(GetIniFileString(inifilename, "General", "StartWidth", "0"), out W);
        int.TryParse(GetIniFileString(inifilename, "General", "StartHeight", "0"), out H);
        inisettings.StartSize = new Size(W, H);
        bool.TryParse(GetIniFileString(inifilename, "General", "ShowPreview", "True"), out inisettings.ShowPreview);
        inisettings.LastPalette = GetIniFileString(inifilename, "General", "LastPalette", "");
        try
        {
            inisettings.DefaultCompression = (SHP_TS_EncodingFormat)Enum.Parse(typeof(SHP_TS_EncodingFormat), GetIniFileString(inifilename, "General", "DefaultCompression", "True"));
        }
        catch 
        { 
        }

        bool.TryParse(GetIniFileString(inifilename, "General", "CreateImages", "False"), out inisettings.CreateImages);
        inisettings.CreateImages_FileName = GetIniFileString(inifilename, "General", "CreateImages_FileName", "tmpimg");
        inisettings.CreateImages_Format = GetIniFileString(inifilename, "General", "CreateImages_Format", "Png");
        bool.TryParse(GetIniFileString(inifilename, "General", "PreventTSWobbleBug", "False"), out inisettings.PreventTSWobbleBug);

        bool.TryParse(GetIniFileString(inifilename, "General", "OptimizeCanvas", "True"), out inisettings.OptimizeCanvas);
        bool.TryParse(GetIniFileString(inifilename, "General", "KeepCentered", "True"), out inisettings.KeepCentered);

        inisettings.RadarColor = GetColorFromHexString(GetIniFileString(inifilename, "General", "RadarColor", "#FFFFFF"));
        bool.TryParse(GetIniFileString(inifilename, "General", "AverageRadarColor", "True"), out inisettings.AverageRadarColor);

        bool.TryParse(GetIniFileString(inifilename, "General", "UseCustomBackgroundColor", "False"), out inisettings.UseCustomBackgroundColor);
        inisettings.CustomBackgroundColor = GetColorFromHexString(GetIniFileString(inifilename, "General", "CustomBackgroundColor", "#FFFFFF"));
        bool.TryParse(GetIniFileString(inifilename, "General", "CombineTransparentPixel", "False"), out inisettings.CombineTransparentPixel);


        inisettings.OutputFolder = GetIniFileString(inifilename, "General", "OutputFolder", "");
        inisettings.PreviewBackgroundImage = GetIniFileString(inifilename, "General", "PreviewBackgroundImage", "");

        inisettings.LastFireFLHFinderDirectory = GetIniFileString(inifilename, "General", "LastFireFLHFinderDirectory", "");
    }


    public static System.Drawing.Color GetSystemDrawingColorFromHexString(string hexString)
    {
        if ((hexString == null) || (hexString == ""))
            hexString = "#FF000000";
        if (!System.Text.RegularExpressions.Regex.IsMatch(hexString, @"[#]([0-9]|[a-f]|[A-F]){8}\b"))
            throw new ArgumentException();

        int alpha = int.Parse(hexString.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
        int red = int.Parse(hexString.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
        int green = int.Parse(hexString.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
        int blue = int.Parse(hexString.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);
        return System.Drawing.Color.FromArgb(alpha, red, green, blue);
    }

    public static string ColorToStr(Color c, bool AsHexString)
    {
        if (AsHexString)
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        else
            return c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString();
    }

    private static Color GetColorFromHexString(string hex)
    {
        try
        {
            return ColorTranslator.FromHtml(hex);
        }
        catch { return Color.FromArgb(0, 0, 0); }
    }

    private static string Color2String(Color c)
    {
        return "[" + c.A.ToString("D3") + "," + c.R.ToString("D3") + "," + c.G.ToString("D3") + "," + c.B.ToString("D3") + "]";
    }

    private static Color String2Color(string s)
    {
        if ((s.StartsWith("[")) && (s.EndsWith("]")) && (s.Split(',').Length == 4))
        {
            s = s.Replace("[", "").Replace("]", "");
            string[] v = s.Split(',');
            byte a = byte.Parse(v[0]);
            byte r = byte.Parse(v[1]);
            byte g = byte.Parse(v[2]);
            byte b = byte.Parse(v[3]);
            return Color.FromArgb(a, r, g, b);
        }
        else return Color.FromArgb(255, 0, 0, 0);
    }

    internal static void SavePaletteSetup(string filename, CPalette[] Palettes)
    {
        var ini = _inis.GetOrOpenOrCreate(filename);
        for (int i = 0; i < Palettes.Length; i++)
        {
            var section = ini.GetOrAddSection("Palette" + i.ToString("D5"));
            section.Add("PaletteFile", Palettes[i].PaletteFile);
            section.Add("ConversionMethod", ((int)Palettes[i].ConversionMethod).ToString());
            section.Add("PaletteName", Palettes[i].PaletteName.ToString());
            for (int c = 0; c < Palettes[i].palette.Length; c++)
                section.Add("Color" + c.ToString("D3"), Palettes[i].palette[c].IsUsed.ToString() + "|" + Palettes[i].palette[c].MakeTransparent.ToString());
        }
        _inis.Save(filename);
    }
    internal static CPalette[] LoadPaletteSetup(string filename)
    {
        CPalette[] pals = new CPalette[0];
        List<string> paletteSections = GetSections(filename);
        paletteSections.RemoveAll(u => !u.Contains("Palette"));
        pals = new CPalette[paletteSections.Count()];
        for (int i = 0; i < paletteSections.Count(); i++)
        {
            CPalette p = new CPalette();
            string palfile = GetIniFileString(filename, paletteSections[i], "PaletteFile", "");
            if (System.IO.File.Exists(palfile))
                p.Load(palfile);
            int cm = 0;
            int.TryParse(GetIniFileString(filename, paletteSections[i], "ConversionMethod", ""), out cm);
            p.ConversionMethod = (ColorConversionMethod)cm;
            p.PaletteName = GetIniFileString(filename, paletteSections[i], "PaletteName", "");

            List<string> keys = GetKeys(filename, paletteSections[i]);
            keys.RemoveAll(u => !u.Contains("Color"));

            for (int c = 0; c < keys.Count(); c++)
            {
                int keynr = 0;
                int.TryParse(keys[c].Replace("Color", ""), out keynr);

                string val = GetIniFileString(filename, paletteSections[i], keys[c], "");

                bool IsUsed = true;
                bool.TryParse(val.Split('|')[0], out IsUsed);
                bool MakeTransparent = true;
                bool.TryParse(val.Split('|')[1], out MakeTransparent);
                if ((keynr >= 0) && (keynr < p.palette.Length))
                {
                    p.palette[keynr].IsUsed = IsUsed;
                    p.palette[keynr].MakeTransparent = MakeTransparent;
                }
            }
            pals[i] = p;
        }

        return pals;
    }

    internal static void SaveProject(string filename, Form_ImageShaper.DGVCell[] Cells, CPalette[] Palettes)
    {
        SavePaletteSetup(filename, Palettes);

        var ini = _inis.GetOrOpenOrCreate(filename);

        for (int i=0;i<Cells.Length;i++)
        {
            var cell = ini.GetOrAddSection("Cell" + i.ToString("D5"));
            CImageFile imf = (CImageFile)Cells[i].Value;
            cell.Add("ColumnIndex", Cells[i].ColumnIndex.ToString());
            cell.Add("RowIndex", Cells[i].RowIndex.ToString());
            cell.Add("FileName", imf.FileName);
            cell.Add("BitFlags", ((int)imf.BitFlags).ToString());
            cell.Add("CompressionFormat", ((int)imf.CompressionFormat).ToString());
            cell.Add("UseCustomBackgroundColor", imf.UseCustomBackgroundColor.ToString());
            cell.Add("CustomBackgroundColor", Color2String(imf.CustomBackgroundColor));
            cell.Add("PaletteIndex", imf.PaletteIndex.ToString());
            //cell.Add("PaletteName", imf.PaletteName.ToString());
            cell.Add("IsSHP", imf.IsSHP.ToString());
            cell.Add("SHPFrameNr", imf.SHPFrameNr.ToString());
            cell.Add("RadarColor", Color2String(imf.RadarColor));
            cell.Add("RadarColorAverage", imf.RadarColorAverage.ToString());
            cell.Add("CombineTransparentPixel", imf.CombineTransparentPixel.ToString());
        }
    }

    internal static Form_ImageShaper.DGVCell[] LoadProject(string filename)
    {
        List<string> cellSections = GetSections(filename);
        cellSections.RemoveAll(u => !u.Contains("Cell"));
        Form_ImageShaper.DGVCell[] cells = new Form_ImageShaper.DGVCell[cellSections.Count];
        for (int i = 0; i < cellSections.Count(); i++)
        {
            Form_ImageShaper.DGVCell cell = new Form_ImageShaper.DGVCell();
            int.TryParse(GetIniFileString(filename, cellSections[i], "ColumnIndex", ""), out cell.ColumnIndex);
            int.TryParse(GetIniFileString(filename, cellSections[i], "RowIndex", ""), out cell.RowIndex);

            string palfile = GetIniFileString(filename, cellSections[i], "FileName", "");
            int tmp = 0;
            int.TryParse(GetIniFileString(filename, cellSections[i], "CompressionFormat", ""), out tmp);
            SHP_TS_EncodingFormat CompressionFormat = (SHP_TS_EncodingFormat)tmp;
            tmp = 0;
            int.TryParse(GetIniFileString(filename, cellSections[i], "PaletteIndex", "0"), out tmp);
            CImageFile imf = new CImageFile(palfile, tmp, CompressionFormat);

            tmp = 0; int.TryParse(GetIniFileString(filename, cellSections[i], "BitFlags", "1"), out tmp); imf.BitFlags = (SHP_TS_BitFlags)tmp;
            bool.TryParse(GetIniFileString(filename, cellSections[i], "UseCustomBackgroundColor", ""), out imf.UseCustomBackgroundColor);
            imf.CustomBackgroundColor = String2Color(GetIniFileString(filename, cellSections[i], "CustomBackgroundColor", ""));
            bool.TryParse(GetIniFileString(filename, cellSections[i], "IsSHP", ""), out imf.IsSHP);
            tmp = -1; int.TryParse(GetIniFileString(filename, cellSections[i], "SHPFrameNr", "-1"), out tmp); imf.SHPFrameNr = tmp;
            imf.RadarColor = String2Color(GetIniFileString(filename, cellSections[i], "RadarColor", ""));
            bool.TryParse(GetIniFileString(filename, cellSections[i], "RadarColorAverage", ""), out imf.RadarColorAverage);
            bool.TryParse(GetIniFileString(filename, cellSections[i], "CombineTransparentPixel", ""), out imf.CombineTransparentPixel);

            cell.Value = imf;
            cells[i] = cell;
        }
        return cells;
    }

}//end of class Cinimanager
