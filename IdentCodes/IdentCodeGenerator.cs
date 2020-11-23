﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Part = Tekla.Structures.Model.Part;

namespace IdentCodes
{
    public static class IdentCodeGenerator
    {
        private enum ProfileType
        {
            Plate,
            Tube,
            HSection,
            Channel,
        }
        public static void GenerateCodes(List<Part> parts)
        {
            foreach (var part in parts)
                GenerateCode(part);
        }

        private static void GenerateCode(Part part)
        {
            var identCode = $"I{GetMaterialClassCode(part)}{GetMaterialTypeCode(part)}" +
                $"{GetToughnessCode(part)}{GetRandomNumberForPlate(part)}" +
                $"{GetMaterialCode(part)}{GetProfileCode(part)}";
            WriteCode(part, identCode);
        }


        #region парсинг свойств детали
        private static string GetPartClass(Part part)
        {
            var partClassPropertyName = "KGCM_PART_STRUCTURE";
            var partClassProperty = "";
            part.GetReportProperty(partClassPropertyName, ref partClassProperty);
            if (partClassProperty != "PS" &&
                partClassProperty != "SS" &&
                partClassProperty != "TS")
                throw new Exception("Неверно заполнен атрибут КГЦМ part structure");
            return partClassProperty;
        }

        private static ProfileType GetProfileType(Part part)
        {
            var profileTypePropertyName = "PROFILE_TYPE";
            var profileTypeProperty = "";
            part.GetReportProperty(profileTypePropertyName, ref profileTypeProperty);
            return ProfTypePropToEnum[profileTypeProperty];
        }

        private static readonly Dictionary<string, ProfileType> ProfTypePropToEnum =
            new Dictionary<string, ProfileType>
        {
            { "B", ProfileType.Plate },
            { "I", ProfileType.HSection },
            { "RO", ProfileType.Tube },
            { "RU", ProfileType.Tube },
            { "U", ProfileType.Channel },
        };

        private static Tuple<string, string> GetSteelGrade(Part part)
        {
            var materialString = part.Material.MaterialString;
            var steelGradeParts = materialString.Split(' ');
            var firstPart = steelGradeParts[0];
            var zParts = steelGradeParts.Where(p => p.ToLower().Contains("z35"));
            var secondPart = zParts.Count() > 0 ? zParts.First() : "";
            return new Tuple<string, string>(firstPart, secondPart);
        }

        private static double GetPlateThickness(Part part)
        {
            var thicknessPropertyName = "WIDTH";
            var thickness = 0.0;
            part.GetReportProperty(thicknessPropertyName, ref thickness);
            return Math.Round(thickness, 4);
        }

        private static double GetTubeDiameter(Part part)
        {
            var diameterPropertyName = "PROFILE.DIAMETER";
            var diameter = 0.0;
            part.GetReportProperty(diameterPropertyName, ref diameter);
            return Math.Round(diameter, 1);
        }

        private static double GetTubeThickness(Part part)
        {
            var thicknessPropertyName = "PROFILE.PLATE_THICKNESS";
            var thickness = 0.0;
            part.GetReportProperty(thicknessPropertyName, ref thickness);
            return Math.Round(thickness, 2);
        }

        private static double GetWebThickness(Part part)
        {
            var webThicknessPropertyName = "PROFILE.WEB_THICKNESS";
            var webThickness = 0.0;
            part.GetReportProperty(webThicknessPropertyName, ref webThickness);
            return Math.Round(webThickness, 1);
        }

        private static double GetProfileHeight(Part part)
        {
            var profileHeightPropertyName = "PROFILE.HEIGHT";
            var profileHeight = 0.0;
            part.GetReportProperty(profileHeightPropertyName, ref profileHeight);
            return Math.Round(profileHeight, 1);
        }
        #endregion

        #region генерация частей ident-кода
        private static string GetMaterialClassCode(Part part)
        {
            var partClass = GetPartClass(part);
            return MaterialClassCodes[partClass];
        }

        private static Dictionary<string, string> MaterialClassCodes =
            new Dictionary<string, string>
            {
                { "PS", "P" },
                { "SS", "S" },
                { "TS", "T" },
            };

        private static int GetMaterialTypeCode(Part part)
        {
            var partType = GetProfileType(part);
            return MaterialTypeCodes[partType];
        }

        private static Dictionary<ProfileType, int> MaterialTypeCodes =
            new Dictionary<ProfileType, int>
            {
                { ProfileType.Plate, 1 },
                { ProfileType.Tube, 2 },
                { ProfileType.HSection, 3 },
                { ProfileType.Channel, 4 },
            };

        private static int GetToughnessCode(Part part)
        {
            var materialGrade = GetSteelGrade(part).Item2;
            return materialGrade.Length == 0 ? 0 : 1;
        }

        private static string GetRandomNumberForPlate(Part part)
        {
            var profileType = GetProfileType(part);
            if (profileType == ProfileType.Plate)
                return "0";
            else
                return "";
        }

        private static int GetMaterialCode(Part part)
        {
            var materialGrade = GetSteelGrade(part).Item1;
            if (materialGrade.Contains('-'))
            {
                materialGrade = materialGrade.Substring(0, materialGrade.IndexOf('-'));
            }
            return MaterialCodes[materialGrade];
        }

        private static Dictionary<string, int> MaterialCodes =
            new Dictionary<string, int>
            {
                { "C355", 0 },
                { "C355-6", 0 },
                { "C355Б-KCV-40", 0 },
                { "C345", 1 },
                { "S420", 2 },
                { "API5L", 3 },
                { "S355", 4 },
                { "С255", 5 },
                { "С355Б", 6 },
            };

        private static string GetProfileCode(Part part)
        {
            var profileType = GetProfileType(part);
            if (profileType == ProfileType.Plate)
                return Math.Round(GetPlateThickness(part)).ToString();
            else if (profileType == ProfileType.Tube)
                return Math.Round(GetTubeDiameter(part)).ToString()
                    + Math.Round(GetTubeThickness(part)).ToString();
            else if (profileType == ProfileType.Channel || profileType == ProfileType.HSection)
                return Math.Round(GetProfileHeight(part)).ToString()
                    + Math.Round(GetWebThickness(part)).ToString();
            else throw new Exception("Отсутствует код для заданного типа сечения");
        }
        #endregion

        private static void WriteCode(Part part, string code)
        {
            part.SetUserProperty("M_IDENT_CODE", code);
        }
    }
}
