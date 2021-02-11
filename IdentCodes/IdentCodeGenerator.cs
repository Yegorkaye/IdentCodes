using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Angle,
            Cone,
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
            if (part.Profile.ProfileString.Contains("EPD"))
                return ProfileType.Cone;
            if (ProfTypePropToEnum.ContainsKey(profileTypeProperty))
                return ProfTypePropToEnum[profileTypeProperty];
            else
                throw new Exception("Заданный тип профиля не поддерживается");

        }

        private static readonly Dictionary<string, ProfileType> ProfTypePropToEnum =
            new Dictionary<string, ProfileType>
        {
            { "B", ProfileType.Plate },
            { "I", ProfileType.HSection },
            { "RO", ProfileType.Tube },
            { "RU", ProfileType.Tube },
            { "U", ProfileType.Channel },
            { "L", ProfileType.Angle },
        };

        private static Tuple<string, string> GetSteelGrade(string materialString)
        {
            var steelGradeParts = Regex.Split(materialString, @"[- ]");
            var firstPart = steelGradeParts[0];
            var zParts = steelGradeParts.Where(p => p.ToLower().Contains("z35"));
            var secondPart = zParts.Count() > 0 ? zParts.First() : "";
            return new Tuple<string, string>(firstPart, secondPart);
        }

        public static string GetMaterialStringFromPart(Part part)
        {
            return part.Material.MaterialString;
        }

        private static double GetPlateThickness(Part part)
        {
            var profile = part.Profile.ProfileString;
            var thickness = 0D;
            if (!profile.Contains('*'))
            {
                thickness = double.Parse(profile.Substring(2));
            }
            else
            {
                var thicknesList = profile.Substring(2).Split('*')
                    .Select(a => { var s = double.TryParse(a, out double r); return s ? r : double.NaN; })
                    .Where(a => a != double.NaN);
                thickness = thicknesList.Min();
            }
            //var thicknessPropertyName = "WIDTH";
            //var thickness = 0.0;
            //part.GetReportProperty(thicknessPropertyName, ref thickness);
            return Math.Round(thickness, 4);
        }

        private static double GetTubeDiameter(Part part)
        {
            var diameterPropertyName = "PROFILE.DIAMETER";
            var diameter = 0.0;
            part.GetReportProperty(diameterPropertyName, ref diameter);
            return Math.Round(diameter, 1);
        }

        private static double GetMaxConeDiameter(Part part)
        {
            return GetConeDiameters(part).Max();
        }

        private static double GetMinConeDiameter(Part part)
        {
            return GetConeDiameters(part).Min();
        }

        private static List<double> GetConeDiameters(Part part)
        {
            var firstDiameterPropertyName = "PROFILE.MAJOR_AXIS_LENGTH_1";
            var secondDiameterPropertyName = "PROFILE.MAJOR_AXIS_LENGTH_2";
            var firstDiameter = 0D;
            var secondDiameter = 0D;
            part.GetReportProperty(firstDiameterPropertyName, ref firstDiameter);
            part.GetReportProperty(secondDiameterPropertyName, ref secondDiameter);
            return new List<double> { firstDiameter, secondDiameter };
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

        private static double GetAngleThickness(Part part)
        {
            var flangeThicknessPropertyName = "PROFILE.FLANGE_THICKNESS_1";
            var flangeThickness = 0.0;
            part.GetReportProperty(flangeThicknessPropertyName, ref flangeThickness);
            return Math.Round(flangeThickness, 1);
        }

        private static double GetProfileHeight(Part part)
        {
            var profileHeightPropertyName = "PROFILE.HEIGHT";
            var profileHeight = 0.0;
            part.GetReportProperty(profileHeightPropertyName, ref profileHeight);
            return Math.Round(profileHeight, 1);
        }

        private static double GetProfileWidth(Part part)
        {
            var profileWidthPropertyName = "PROFILE.WIDTH";
            var profileWidth = 0.0;
            part.GetReportProperty(profileWidthPropertyName, ref profileWidth);
            return Math.Round(profileWidth, 1);
        }
        #endregion

        #region генерация частей ident-кода
        public static string GetMaterialClassCode(Part part)
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

        public static int GetMaterialTypeCode(Part part)
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
                { ProfileType.Angle, 5 },
                { ProfileType.Cone, 6 },
            };

        public static int GetToughnessCode(string materialString)
        {
            var materialGrade = GetSteelGrade(materialString).Item2;
            return materialGrade.Length == 0 ? 0 : 1;
        }

        public static string GetRandomNumberForPlate(Part part)
        {
            var profileType = GetProfileType(part);
            if (profileType == ProfileType.Plate)
                return "0";
            else if (profileType == ProfileType.Angle)
                return GetNumberForAngles(part).ToString();
            else
                return "";
        }

        private static int GetNumberForAngles(Part part)
        {
            if (GetProfileHeight(part).CompareTo(GetProfileWidth(part)) == 0)
                return 0;
            else
                return 1;
        }

        public static int GetMaterialCode(string materialString)
        {
            var materialGrade = GetSteelGrade(materialString).Item1.Replace('С', 'C');

            foreach (var acceptableGrade in MaterialCodes.Keys)
            {
                if (materialGrade.Contains(acceptableGrade))
                {
                    return MaterialCodes[acceptableGrade];
                }
            }

            throw new Exception("Материал не найден");
        }

        private static Dictionary<string, int> MaterialCodes =
            new Dictionary<string, int>
            {
                { "C355Б", 6 },
                { "C355", 0 },
                { "C345", 1 },
                { "S420", 2 },
                { "API5L", 3 },
                { "S355", 4 },
                { "C255", 5 },
            };

        public static string GetProfileCode(Part part)
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
            else if (profileType == ProfileType.Angle)
                return Math.Round(GetProfileHeight(part)).ToString() + Math.Round(GetAngleThickness(part)).ToString();
            else if (profileType == ProfileType.Cone)
                return Math.Round(GetMaxConeDiameter(part)).ToString() + Math.Round(GetMinConeDiameter(part)).ToString()
                    + Math.Round(GetTubeThickness(part));
            else throw new Exception("Отсутствует код для заданного типа сечения");
        }
        #endregion
    }
}
