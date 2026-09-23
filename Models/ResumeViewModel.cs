using System.Collections.Generic;

namespace ErJobPortal.Models
{
    public class ResumeViewModel
    {
        // =========================================================
        // CANDIDATE PROFILE
        // =========================================================

        public CandidateProfileModel Profile { get; set; }
            = new CandidateProfileModel();


        // =========================================================
        // DROPDOWN DATA
        // =========================================================

        public List<DropdownModel> Relationships { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> Streams { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> Divisions { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> InternshipFellowshipType { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> InternshipTitles { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> InternshipDurations { get; set; }
            = new List<DropdownModel>();

        public List<DropdownModel> InternshipStatuses { get; set; }
            = new List<DropdownModel>();


        // =========================================================
        // CANDIDATE DETAILS
        // =========================================================

        public int CandidateID { get; set; }

        public string CandidateName { get; set; } = "";

        public string CandidateEmail { get; set; } = "";

        public string CandidatePhone { get; set; } = "";


        // =========================================================
        // PERSONAL DETAILS
        // =========================================================

        public string ProfilePic { get; set; } = "";

        public string Sign { get; set; } = "";

        public string Gender { get; set; } = "";

        public bool Divyang { get; set; }


        // =========================================================
        // CONTACT DETAILS
        // =========================================================

        public string Address { get; set; } = "";

        public string Country { get; set; } = "";

        public string State { get; set; } = "";

        public string City { get; set; } = "";

        public string Pincode { get; set; } = "";


        // =========================================================
        // RESUME DETAILS
        // =========================================================

        public string Objective { get; set; } = "";

        public string Aim { get; set; } = "";


        // =========================================================
        // EDUCATION DETAILS
        // =========================================================

        // SSC
        public int? SSCYear { get; set; }

        public string SSCDivision { get; set; } = "";


        // HSC / DIPLOMA
        public int? HSCDiplomaYear { get; set; }

        public string HSCDiplomaDivision { get; set; } = "";


        // GRADUATION
        public int? GraduationYear { get; set; }

        public string GraduationDivision { get; set; } = "";

        public string GraduationStream { get; set; } = "";

        public string OtherGraduationStream { get; set; } = "";


        // POST GRADUATION
        public int? PGYear { get; set; }

        public string PGDivision { get; set; } = "";

        public string PGStream { get; set; } = "";

        public string OtherPGSpecialization { get; set; } = "";


        // =========================================================
        // PHD DETAILS
        // =========================================================

        public int? PhDYear { get; set; }

        public string PhDStatus { get; set; } = "";

        public string PhDTopic { get; set; } = "";

        public string PreviousPhDTopic { get; set; } = "";

        public int? PreviousPhDYear { get; set; }


        // =========================================================
        // LANGUAGES
        // Minimum 2 / Maximum 6 for Resume Type-1
        // =========================================================

        public string Language1 { get; set; } = "";

        public string Language2 { get; set; } = "";

        public string Language3 { get; set; } = "";

        public string Language4 { get; set; } = "";

        public string Language5 { get; set; } = "";

        public string Language6 { get; set; } = "";


        // =========================================================
        // SKILLS
        // =========================================================

        public string SkillType { get; set; } = "";

        public string AdministrativeSkills { get; set; } = "";

        public string MedicalSkills { get; set; } = "";

        public string TechnicalSkills { get; set; } = "";


        // =========================================================
        // REFERENCES
        // =========================================================

        public string Reference1 { get; set; } = "";

        public string Reference2 { get; set; } = "";


        // =========================================================
        // INTERNSHIP / FELLOWSHIP
        // =========================================================

        public string InternshipType { get; set; } = "";

        public string PreferredCountry { get; set; } = "";

        public string PreferredState { get; set; } = "";

        public string PreferredCity { get; set; } = "";


        // =========================================================
        // PREVIOUS INTERNSHIP - 1
        // =========================================================

        public string Internship1Organization { get; set; } = "";

        public string Internship1Title { get; set; } = "";

        public string Internship1Duration { get; set; } = "";

        public string Internship1Status { get; set; } = "";


        // =========================================================
        // PREVIOUS INTERNSHIP - 2
        // =========================================================

        public string Internship2Organization { get; set; } = "";

        public string Internship2Title { get; set; } = "";

        public string Internship2Duration { get; set; } = "";

        public string Internship2Status { get; set; } = "";


        // =========================================================
        // PREVIOUS INTERNSHIP - 3
        // =========================================================

        public string Internship3Organization { get; set; } = "";

        public string Internship3Title { get; set; } = "";

        public string Internship3Duration { get; set; } = "";

        public string Internship3Status { get; set; } = "";


        // =========================================================
        // PREVIOUS INTERNSHIP - 4
        // =========================================================

        public string Internship4Organization { get; set; } = "";

        public string Internship4Title { get; set; } = "";

        public string Internship4Duration { get; set; } = "";

        public string Internship4Status { get; set; } = "";


        // =========================================================
        // ACHIEVEMENTS
        // =========================================================

        public string Achievement1 { get; set; } = "";

        public string Achievement2 { get; set; } = "";

        public string Achievement3 { get; set; } = "";


        // =========================================================
        // HOBBIES
        // =========================================================

        public string Hobby1 { get; set; } = "";

        public string Hobby2 { get; set; } = "";

        public string Hobby3 { get; set; } = "";


        // =========================================================
        // SOCIAL LINKS
        // =========================================================

        public string GitHub { get; set; } = "";

        public string LinkedIn { get; set; } = "";


        // =========================================================
        // DOCUMENTS
        // =========================================================

        public string ResumeFile { get; set; } = "";

        public string PhotoFile { get; set; } = "";

        public string SignatureFile { get; set; } = "";


        // =========================================================
        // RESUME PROFILE / TYPE
        // =========================================================

        public int? ResumeProfile { get; set; }

        public int ResumeType { get; set; }
    }
}