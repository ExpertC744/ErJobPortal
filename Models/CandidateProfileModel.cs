using System.ComponentModel.DataAnnotations;


namespace ErJobPortal.Models
{
    public class CandidateProfileModel
    {
        //   public int nID { get; set;  }
        //   public int CandidateID { get; set; }

        //   // Address
        //   public string? CountryID { get; set; }
        //   public string? StateID { get; set; }
        //   public string? CityID { get; set; }

        //   [RegularExpression(
        //    @"^[0-9]{6}$",
        //    ErrorMessage = "Pincode must contain exactly 6 digits."
        //)]
        //   public string? Pincode { get; set; }

        //   // Education

        //   public int? SSC_YEAR { get; set; }
        //   public int? SSC_DIVISION { get; set; }

        //   public int? HSC_DIPLOMA_YEAR { get; set; }
        //   public int? HSC_DIPLOMA_DIVISION { get; set; }

        //   public int? Graduation_Year { get; set; }
        //   public int? Graduation_Division { get; set; }
        //   public int? Graduation_Stream { get; set; }
        //   public string? Other_Stream { get; set; }

        //   public int? PG_Year { get; set; }
        //   public int? PG_Division { get; set; }
        //   public int? PG_Stream { get; set; }
        //   public string? Other_Specialization { get; set; }

        //   public int? PhD_Year { get; set; }
        //   public int? PhD_Status { get; set; }
        //   public string? PhD_Topic { get; set; }
        //   public string? Previous_PhD_Topic_Year { get; set; }

        //   //Internship_FellowshipType
        //   public int? Internship_FellowshipType { get; set; }
        //   public string? Preferred_Country { get; set; }
        //   public string? Preferred_State { get; set; }
        //   public string? Preferred_City { get; set; }

        //   // DOCUMENTS
        //   public string? sResume { get; set; }
        //   public string? sPhoto { get; set; }
        //   public string? sSignature { get; set; }
        //   public string? sDivyang { get; set; }

        //   // HOBBIES
        //   public string? sHobbies { get; set; }

        //   // INTERNSHIP DETAILS
        //   public string? sOrgName1 { get; set; }
        //   public int? sOrgIntTitle1 { get; set; }
        //   public int? sOrgIntDuration1 { get; set; }
        //   public int? sOrgIntStatus1 { get; set; }
        //   public string? sOrgName2 { get; set; }
        //   public int? sOrgIntTitle2 { get; set; }
        //   public int? sOrgIntDuration2 { get; set; }
        //   public int? sOrgIntStatus2 { get; set; }
        //   public string? sOrgName3 { get; set; }
        //   public int? sOrgIntTitle3 { get; set; }
        //   public int? sOrgIntDuration3 { get; set; }
        //   public int? sOrgIntStatus3 { get; set; }
        //   public string? sOrgName4 { get; set; }
        //   public int? sOrgIntTitle4 { get; set; }
        //   public int? sOrgIntDuration4 { get; set; }
        //   public int? sOrgIntStatus4 { get; set; }

        //   // Languages Known
        //   public string? sLanguage1 { get; set; }
        //   public string? sLanguage2 { get; set; }
        //   public string? sLanguage3 { get; set; }
        //   public string? sLanguage4 { get; set; }
        //   public string? sLanguage5 { get; set; }
        //   public string? sLanguage6 { get; set; }
        //   public string? sLanguage7 { get; set; }
        //   public string? sLanguage8 { get; set; }
        //   public int? sLanguageStar1 { get; set; }
        //   public int? sLanguageStar2 { get; set; }
        //   public int? sLanguageStar3 { get; set; }
        //   public int? sLanguageStar4 { get; set; }
        //   public int? sLanguageStar5 { get; set; }
        //   public int? sLanguageStar6 { get; set; }
        //   public int? sLanguageStar7 { get; set; }
        //   public int? sLanguageStar8 { get; set; }

        //   // Reference
        //   public string? sRefName1 { get; set; }
        //   public string? sRefName2 { get; set; }
        //   public int? sRefRelationName1 { get; set; }
        //   public int? sRefRelationName2 { get; set; }
        //   public string? sLocation1 { get; set; }
        //   public string? sLocation2 { get; set; }
        //   public string? sMobile1 { get; set; }
        //   public string? sMobile2 { get; set; }

        //   // Achievements / Certification
        //   public string? Achievements_Certification1 { get; set; }
        //   public string? Achievements_Certification2 { get; set; }
        //   public string? Achievements_Certification3 { get; set; }

        //   //Add Links
        //   public string? GitHub { get; set; }
        //   public string? Linkedin { get; set; }

        //   // Objective
        //   public string? Objective { get; set; }
        //   public int? Resume_Profile { get; set; }

        //   //Skills
        //   public string? sMedicalSkillIDs { get; set; }
        //   public string? sMedicalSkillStarIDs { get; set; }
        //   public string? sTechnicalSkillIDs { get; set; }
        //   public string? sTechnicalSkillStarIDs { get; set; }
        //   public string? sNonTechnicalSkillIDs { get; set; }
        //   public string? sNonTechnicalSkillStarIDs { get; set; }

        public int nID { get; set; }

        public int CandidateID { get; set; }

        // =========================================================
        // ADDRESS
        // =========================================================

        [Required(ErrorMessage = "Please select country.")]
        public string? CountryID { get; set; }

        [Required(ErrorMessage = "Please select state.")]
        public string? StateID { get; set; }

        [Required(ErrorMessage = "Please select city.")]
        public string? CityID { get; set; }

        [Required(ErrorMessage = "Please enter pincode.")]
        [RegularExpression(
            @"^[0-9]{6}$",
            ErrorMessage = "Pincode must contain exactly 6 digits."
        )]
        public string? Pincode { get; set; }


        // =========================================================
        // EDUCATION
        // =========================================================

        [Range(1950, 2100, ErrorMessage = "Please enter a valid SSC year.")]
        public int? SSC_YEAR { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select SSC division.")]
        public int? SSC_DIVISION { get; set; }


        [Range(1950, 2100, ErrorMessage = "Please enter a valid HSC / Diploma year.")]
        public int? HSC_DIPLOMA_YEAR { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select HSC / Diploma division.")]
        public int? HSC_DIPLOMA_DIVISION { get; set; }


        [Range(1950, 2100, ErrorMessage = "Please enter a valid graduation year.")]
        public int? Graduation_Year { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select graduation division.")]
        public int? Graduation_Division { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select graduation stream.")]
        public int? Graduation_Stream { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Other stream cannot exceed 100 characters."
        )]
        public string? Other_Stream { get; set; }


        [Range(1950, 2100, ErrorMessage = "Please enter a valid PG year.")]
        public int? PG_Year { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select PG division.")]
        public int? PG_Division { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select PG stream.")]
        public int? PG_Stream { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Other specialization cannot exceed 100 characters."
        )]
        public string? Other_Specialization { get; set; }


        [Range(1950, 2100, ErrorMessage = "Please enter a valid PhD year.")]
        public int? PhD_Year { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select PhD status.")]
        public int? PhD_Status { get; set; }

        [StringLength(
            500,
            ErrorMessage = "PhD topic cannot exceed 500 characters."
        )]
        public string? PhD_Topic { get; set; }

        [StringLength(
            200,
            ErrorMessage = "Previous PhD topic / year cannot exceed 200 characters."
        )]
        public string? Previous_PhD_Topic_Year { get; set; }


        // =========================================================
        // INTERNSHIP / FELLOWSHIP PREFERENCE
        // =========================================================

        [Required(ErrorMessage = "Please select internship / fellowship type.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select internship / fellowship type."
        )]
        public int? Internship_FellowshipType { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Preferred country cannot exceed 100 characters."
        )]
        public string? Preferred_Country { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Preferred state cannot exceed 100 characters."
        )]
        public string? Preferred_State { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Preferred city cannot exceed 100 characters."
        )]
        public string? Preferred_City { get; set; }


        // =========================================================
        // DOCUMENTS
        // =========================================================

        public string? sResume { get; set; }

        public string? sPhoto { get; set; }

        public string? sSignature { get; set; }

        [RegularExpression(
            @"^(Yes|No)$",
            ErrorMessage = "Please select Yes or No."
        )]
        public string? sDivyang { get; set; }


        // =========================================================
        // HOBBIES
        // =========================================================

        [StringLength(
            500,
            ErrorMessage = "Hobbies cannot exceed 500 characters."
        )]
        public string? sHobbies { get; set; }


        // =========================================================
        // INTERNSHIP DETAILS
        // =========================================================

        [StringLength(
            200,
            ErrorMessage = "Organization name cannot exceed 200 characters."
        )]
        public string? sOrgName1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship title.")]
        public int? sOrgIntTitle1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship duration.")]
        public int? sOrgIntDuration1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship status.")]
        public int? sOrgIntStatus1 { get; set; }


        [StringLength(
            200,
            ErrorMessage = "Organization name cannot exceed 200 characters."
        )]
        public string? sOrgName2 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship title.")]
        public int? sOrgIntTitle2 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship duration.")]
        public int? sOrgIntDuration2 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship status.")]
        public int? sOrgIntStatus2 { get; set; }


        [StringLength(
            200,
            ErrorMessage = "Organization name cannot exceed 200 characters."
        )]
        public string? sOrgName3 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship title.")]
        public int? sOrgIntTitle3 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship duration.")]
        public int? sOrgIntDuration3 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship status.")]
        public int? sOrgIntStatus3 { get; set; }


        [StringLength(
            200,
            ErrorMessage = "Organization name cannot exceed 200 characters."
        )]
        public string? sOrgName4 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship title.")]
        public int? sOrgIntTitle4 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship duration.")]
        public int? sOrgIntDuration4 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select internship status.")]
        public int? sOrgIntStatus4 { get; set; }


        // =========================================================
        // LANGUAGES
        // =========================================================

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage1 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage2 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage3 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage4 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage5 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage6 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage7 { get; set; }

        [StringLength(
            50,
            ErrorMessage = "Language cannot exceed 50 characters."
        )]
        public string? sLanguage8 { get; set; }


        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar1 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar2 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar3 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar4 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar5 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar6 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar7 { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? sLanguageStar8 { get; set; }


        // =========================================================
        // REFERENCES
        // =========================================================

        [StringLength(
            100,
            ErrorMessage = "Reference name cannot exceed 100 characters."
        )]
        public string? sRefName1 { get; set; }

        [StringLength(
            100,
            ErrorMessage = "Reference name cannot exceed 100 characters."
        )]
        public string? sRefName2 { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please select relationship.")]
        public int? sRefRelationName1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select relationship.")]
        public int? sRefRelationName2 { get; set; }


        [StringLength(
            150,
            ErrorMessage = "Location cannot exceed 150 characters."
        )]
        public string? sLocation1 { get; set; }

        [StringLength(
            150,
            ErrorMessage = "Location cannot exceed 150 characters."
        )]
        public string? sLocation2 { get; set; }


        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Mobile number must contain exactly 10 digits."
        )]
        public string? sMobile1 { get; set; }

        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Mobile number must contain exactly 10 digits."
        )]
        public string? sMobile2 { get; set; }


        // =========================================================
        // ACHIEVEMENTS / CERTIFICATIONS
        // =========================================================

        [StringLength(
            1000,
            ErrorMessage = "Achievement cannot exceed 1000 characters."
        )]
        public string? Achievements_Certification1 { get; set; }

        [StringLength(
            1000,
            ErrorMessage = "Achievement cannot exceed 1000 characters."
        )]
        public string? Achievements_Certification2 { get; set; }

        [StringLength(
            1000,
            ErrorMessage = "Achievement cannot exceed 1000 characters."
        )]
        public string? Achievements_Certification3 { get; set; }


        // =========================================================
        // SOCIAL LINKS
        // =========================================================

        [Url(ErrorMessage = "Please enter a valid GitHub URL.")]
        [StringLength(
            300,
            ErrorMessage = "GitHub URL cannot exceed 300 characters."
        )]
        public string? GitHub { get; set; }

        [Url(ErrorMessage = "Please enter a valid LinkedIn URL.")]
        [StringLength(
            300,
            ErrorMessage = "LinkedIn URL cannot exceed 300 characters."
        )]
        public string? Linkedin { get; set; }


        // =========================================================
        // OBJECTIVE
        // =========================================================

        //[StringLength(
        //    2000,
        //    ErrorMessage = "Objective cannot exceed 2000 characters."
        //)]
        //public string? Objective { get; set; }
        [Required(ErrorMessage = "Career Objective is required.")]
        [StringLength(1000, MinimumLength = 20,
    ErrorMessage = "Career Objective must be between 20 and 1000 characters.")]

        public string? Objective { get; set; }

        public int? Resume_Profile { get; set; }


        // =========================================================
        // SKILLS
        // =========================================================

        public string? sMedicalSkillIDs { get; set; }

        public string? sMedicalSkillStarIDs { get; set; }

        public string? sTechnicalSkillIDs { get; set; }

        public string? sTechnicalSkillStarIDs { get; set; }

        public string? sNonTechnicalSkillIDs { get; set; }

        public string? sNonTechnicalSkillStarIDs { get; set; }



    }
}