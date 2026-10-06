using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class ResumeController : Controller
    {
        private readonly CandidateProfileRepository _repository;

        public ResumeController(CandidateProfileRepository repository)
        {
            _repository = repository;
        }

        // ============================================================
        // COMMON MODEL BUILDER
        // ============================================================
        private ResumeViewModel? BuildResumeModel(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return null;
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),

                InternshipFellowshipType =
                    _repository.GetInternshipFellowshipType(),

                InternshipTitles =
                    _repository.GetInternshipTitles(),

                InternshipDurations =
                    _repository.GetInternshipDurations(),

                InternshipStatuses =
                    _repository.GetInternshipStatuses(),

                MedicalSkills =
                    _repository.GetMedicalSkills(),

                TechnicalSkills =
                    _repository.GetTechnicalSkills(),

                NonTechnicalSkills =
                    _repository.GetNonTechnicalSkills()
            };

            // ========================================================
            // GET REAL CANDIDATE NAME, EMAIL AND PHONE
            // ========================================================
            var candidateAccount =
                _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateID = candidateAccount.CandidateID;

                model.CandidateName =
                    $"{candidateAccount.sFName} {candidateAccount.sLName}"
                    .Trim();

                model.CandidateEmail =
                    candidateAccount.sEmail ?? "";

                model.CandidatePhone =
                    candidateAccount.sMobile ?? "";
            }
            else
            {
                model.CandidateID = id;
                model.CandidateName = "Candidate";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return model;
        }


        // ============================================================
        // COMMON VIEW RENDER METHOD
        // ============================================================
        private IActionResult RenderProfile(int id, string viewName)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var model = BuildResumeModel(id);

            if (model == null)
            {
                return NotFound(
                    $"Candidate profile not found for CandidateID: {id}"
                );
            }

            return View(
                $"~/Views/Resume/{viewName}.cshtml",
                model
            );
        }

        //shrirang 02/10/26
        // ============================================================
        // MAIN RESUME
        // ============================================================
        [HttpGet]
        public IActionResult Resume(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            // ============================================================
            // GET CANDIDATE PROFILE
            // ============================================================

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound(
                    $"Candidate profile not found for CandidateID: {id}"
                );
            }

            // ============================================================
            // GET SELECTED RESUME PROFILE
            // ============================================================

            int resumeProfile = profile.Resume_Profile ?? 1;

            // ============================================================
            // VALIDATE TEMPLATE NUMBER
            // ============================================================

            if (resumeProfile < 1 || resumeProfile > 59)
            {
                resumeProfile = 1;
            }

            // ============================================================
            // OPEN SELECTED TEMPLATE
            // ============================================================

            string actionName = resumeProfile switch
            {
                1 => "ViewProfileOne",
                2 => "ViewProfileTwo",
                3 => "ViewProfileThree",
                4 => "ViewProfileFour",
                5 => "ViewProfileFive",
                6 => "ViewProfileSix",
                7 => "ViewProfileSeven",
                8 => "ViewProfileEight",
                9 => "ViewProfileNine",
                10 => "ViewProfileTen",
                11 => "ViewProfileEleven",
                12 => "ViewProfileTwelve",
                13 => "ViewProfileThirteen",
                14 => "ViewProfileFourteen",
                15 => "ViewProfileFifteen",
                16 => "ViewProfileSixteen",
                17 => "ViewProfileSeventeen",
                18 => "ViewProfileEighteen",
                19 => "ViewProfileNineteen",
                20 => "ViewProfileTwenty",
                21 => "ViewProfileTwentyOne",
                22 => "ViewProfileTwentyTwo",
                23 => "ViewProfileTwentyThree",
                24 => "ViewProfileTwentyFour",
                25 => "ViewProfileTwentyFive",
                26 => "ViewProfileTwentySix",
                27 => "ViewProfileTwentySeven",
                28 => "ViewProfileTwentyEight",
                29 => "ViewProfileTwentyNine",
                30 => "ViewProfileThirty",
                31 => "ViewProfileThirtyOne",
                32 => "ViewProfileThirtyTwo",
                33 => "ViewProfileThirtyThree",
                34 => "ViewProfileThirtyFour",
                35 => "ViewProfileThirtyFive",
                36 => "ViewProfileThirtySix",
                37 => "ViewProfileThirtySeven",
                38 => "ViewProfileThirtyEight",
                39 => "ViewProfileThirtyNine",
                40 => "ViewProfileForty",
                41 => "ViewProfileFortyOne",
                42 => "ViewProfileFortyTwo",
                43 => "ViewProfileFortyThree",
                44 => "ViewProfileFortyFour",
                45 => "ViewProfileFortyFive",
                46 => "ViewProfileFortySix",
                47 => "ViewProfileFortySeven",
                48 => "ViewProfileFortyEight",
                49 => "ViewProfileFortyNine",
                50 => "ViewProfileFifty",
                51 => "ViewProfileFiftyOne",
                52 => "ViewProfileFiftyTwo",
                53 => "ViewProfileFiftyThree",
                54 => "ViewProfileFiftyFour",
                55 => "ViewProfileFiftyFive",
                56 => "ViewProfileFiftySix",
                57 => "ViewProfileFiftySeven",
                58 => "ViewProfileFiftyEight",
                59 => "ViewProfileFiftyNine",
                91 => "ViewProfileNinetyOne",
                92 => "ViewProfileNinetyTwo",
                93 => "ViewProfileNinetyThree",
                94 => "ViewProfileNinetyFour",
                95 => "ViewProfileNinetyFive",
                96 => "ViewProfileNinetySix",
                97 => "ViewProfileNinetySeven",
                98 => "ViewProfileNinetyEight",
                99 => "ViewProfileNinetyNine",
                100 => "ViewProfileOneHundred",

                _ => "Resume"
            };

            // ============================================================
            // REDIRECT TO SELECTED TEMPLATE
            // ============================================================

            return RedirectToAction(
                actionName,
                "Resume",
                new { id = id }
            );
        }


        // ============================================================
        // RESUME TEMPLATE 1
        // ============================================================
        public IActionResult ViewProfileOne(int id)
        {
            return RenderProfile(id, "ViewProfileOne");
        }


        // ============================================================
        // RESUME TEMPLATE 2
        // ============================================================
        public IActionResult ViewProfileTwo(int id)
        {
            return RenderProfile(id, "ViewProfileTwo");
        }


        // ============================================================
        // RESUME TEMPLATE 3
        // ============================================================
        public IActionResult ViewProfileThree(int id)
        {
            return RenderProfile(id, "ViewProfileThree");
        }


        // ============================================================
        // RESUME TEMPLATE 4
        // ============================================================
        public IActionResult ViewProfileFour(int id)
        {
            return RenderProfile(id, "ViewProfileFour");
        }


        // ============================================================
        // RESUME TEMPLATE 5
        // ============================================================
        public IActionResult ViewProfileFive(int id)
        {
            return RenderProfile(id, "ViewProfileFive");
        }


        // ============================================================
        // RESUME TEMPLATE 6
        // ============================================================
        public IActionResult ViewProfileSix(int id)
        {
            return RenderProfile(id, "ViewProfileSix");
        }


        // ============================================================
        // RESUME TEMPLATE 7
        // ============================================================
        public IActionResult ViewProfileSeven(int id)
        {
            return RenderProfile(id, "ViewProfileSeven");
        }


        // ============================================================
        // RESUME TEMPLATE 8
        // ============================================================
        public IActionResult ViewProfileEight(int id)
        {
            return RenderProfile(id, "ViewProfileEight");
        }


        // ============================================================
        // RESUME TEMPLATE 9
        // ============================================================
        public IActionResult ViewProfileNine(int id)
        {
            return RenderProfile(id, "ViewProfileNine");
        }


        // ============================================================
        // RESUME TEMPLATE 10
        // ============================================================
        public IActionResult ViewProfileTen(int id)
        {
            return RenderProfile(id, "ViewProfileTen");
        }


        // ============================================================
        // RESUME TEMPLATE 11
        // ============================================================
        public IActionResult ViewProfileEleven(int id)
        {
            return RenderProfile(id, "ViewProfileEleven");
        }


        // ============================================================
        // RESUME TEMPLATE 12
        // ============================================================
        public IActionResult ViewProfileTwelve(int id)
        {
            return RenderProfile(id, "ViewProfileTwelve");
        }


        // ============================================================
        // RESUME TEMPLATE 13
        // ============================================================
        public IActionResult ViewProfileThirteen(int id)
        {
            return RenderProfile(id, "ViewProfileThirteen");
        }


        // ============================================================
        // RESUME TEMPLATE 14
        // ============================================================
        public IActionResult ViewProfileFourteen(int id)
        {
            return RenderProfile(id, "ViewProfileFourteen");
        }


        // ============================================================
        // RESUME TEMPLATE 15
        // ============================================================
        public IActionResult ViewProfileFifteen(int id)
        {
            return RenderProfile(id, "ViewProfileFifteen");
        }


        // ============================================================
        // RESUME TEMPLATE 16
        // ============================================================
        public IActionResult ViewProfileSixteen(int id)
        {
            return RenderProfile(id, "ViewProfileSixteen");
        }


        // ============================================================
        // RESUME TEMPLATE 17
        // ============================================================
        public IActionResult ViewProfileSeventeen(int id)
        {
            return RenderProfile(id, "ViewProfileSeventeen");
        }


        // ============================================================
        // RESUME TEMPLATE 18
        // ============================================================
        public IActionResult ViewProfileEighteen(int id)
        {
            return RenderProfile(id, "ViewProfileEighteen");
        }


        // ============================================================
        // RESUME TEMPLATE 19
        // ============================================================
        public IActionResult ViewProfileNineteen(int id)
        {
            return RenderProfile(id, "ViewProfileNineteen");
        }


        // ============================================================
        // RESUME TEMPLATE 20
        // ============================================================
        public IActionResult ViewProfileTwenty(int id)
        {
            return RenderProfile(id, "ViewProfileTwenty");
        }


        // ============================================================
        // RESUME TEMPLATE 21
        // ============================================================
        public IActionResult ViewProfileTwentyOne(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyOne");
        }


        // ============================================================
        // RESUME TEMPLATE 22
        // ============================================================
        public IActionResult ViewProfileTwentyTwo(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyTwo");
        }


        // ============================================================
        // RESUME TEMPLATE 23
        // ============================================================
        public IActionResult ViewProfileTwentyThree(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyThree");
        }


        // ============================================================
        // RESUME TEMPLATE 24
        // ============================================================
        public IActionResult ViewProfileTwentyFour(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyFour");
        }


        // ============================================================
        // RESUME TEMPLATE 25
        // ============================================================
        public IActionResult ViewProfileTwentyFive(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyFive");
        }


        // ============================================================
        // RESUME TEMPLATE 26
        // ============================================================
        public IActionResult ViewProfileTwentySix(int id)
        {
            return RenderProfile(id, "ViewProfileTwentySix");
        }


        // ============================================================
        // RESUME TEMPLATE 27
        // ============================================================
        public IActionResult ViewProfileTwentySeven(int id)
        {
            return RenderProfile(id, "ViewProfileTwentySeven");
        }


        // ============================================================
        // RESUME TEMPLATE 28
        // ============================================================
        public IActionResult ViewProfileTwentyEight(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyEight");
        }


        // ============================================================
        // RESUME TEMPLATE 29
        // ============================================================
        public IActionResult ViewProfileTwentyNine(int id)
        {
            return RenderProfile(id, "ViewProfileTwentyNine");
        }


        // ============================================================
        // RESUME TEMPLATE 30
        // ============================================================
        public IActionResult ViewProfileThirty(int id)
        {
            return RenderProfile(id, "ViewProfileThirty");
        }


        // ============================================================
        // RESUME TEMPLATE 31
        // ============================================================
        public IActionResult ViewProfileThirtyOne(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyOne");
        }


        // ============================================================
        // RESUME TEMPLATE 32
        // ============================================================
        public IActionResult ViewProfileThirtyTwo(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyTwo");
        }


        // ============================================================
        // RESUME TEMPLATE 33
        // ============================================================
        public IActionResult ViewProfileThirtyThree(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyThree");
        }


        // ============================================================
        // RESUME TEMPLATE 34
        // ============================================================
        public IActionResult ViewProfileThirtyFour(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyFour");
        }


        // ============================================================
        // RESUME TEMPLATE 35
        // ============================================================
        public IActionResult ViewProfileThirtyFive(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyFive");
        }


        // ============================================================
        // RESUME TEMPLATE 36
        // ============================================================
        public IActionResult ViewProfileThirtySix(int id)
        {
            return RenderProfile(id, "ViewProfileThirtySix");
        }


        // ============================================================
        // RESUME TEMPLATE 37
        // ============================================================
        public IActionResult ViewProfileThirtySeven(int id)
        {
            return RenderProfile(id, "ViewProfileThirtySeven");
        }


        // ============================================================
        // RESUME TEMPLATE 38
        // ============================================================
        public IActionResult ViewProfileThirtyEight(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyEight");
        }


        // ============================================================
        // RESUME TEMPLATE 39
        // ============================================================
        public IActionResult ViewProfileThirtyNine(int id)
        {
            return RenderProfile(id, "ViewProfileThirtyNine");
        }


        // ============================================================
        // RESUME TEMPLATE 40
        // ============================================================
        public IActionResult ViewProfileForty(int id)
        {
            return RenderProfile(id, "ViewProfileForty");
        }
        // ============================================================
        // RESUME TEMPLATE 41
        // ============================================================
        public IActionResult ViewProfileFortyOne(int id)
        {
            return RenderProfile(id, "ViewProfileFortyOne");
        }


        // ============================================================
        // RESUME TEMPLATE 42
        // ============================================================
        public IActionResult ViewProfileFortyTwo(int id)
        {
            return RenderProfile(id, "ViewProfileFortyTwo");
        }


        // ============================================================
        // RESUME TEMPLATE 43
        // ============================================================
        public IActionResult ViewProfileFortyThree(int id)
        {
            return RenderProfile(id, "ViewProfileFortyThree");
        }


        // ============================================================
        // RESUME TEMPLATE 44
        // ============================================================
        public IActionResult ViewProfileFortyFour(int id)
        {
            return RenderProfile(id, "ViewProfileFortyFour");
        }


        // ============================================================
        // RESUME TEMPLATE 45
        // ============================================================
        public IActionResult ViewProfileFortyFive(int id)
        {
            return RenderProfile(id, "ViewProfileFortyFive");
        }


        // ============================================================
        // RESUME TEMPLATE 46
        // ============================================================
        public IActionResult ViewProfileFortySix(int id)
        {
            return RenderProfile(id, "ViewProfileFortySix");
        }


        // ============================================================
        // RESUME TEMPLATE 47
        // ============================================================
        public IActionResult ViewProfileFortySeven(int id)
        {
            return RenderProfile(id, "ViewProfileFortySeven");
        }


        // ============================================================
        // RESUME TEMPLATE 48
        // ============================================================
        public IActionResult ViewProfileFortyEight(int id)
        {
            return RenderProfile(id, "ViewProfileFortyEight");
        }


        // ============================================================
        // RESUME TEMPLATE 49
        // ============================================================
        public IActionResult ViewProfileFortyNine(int id)
        {
            return RenderProfile(id, "ViewProfileFortyNine");
        }


        // ============================================================
        // RESUME TEMPLATE 50
        // ============================================================
        public IActionResult ViewProfileFifty(int id)
        {
            return RenderProfile(id, "ViewProfileFifty");
        }


        // ============================================================
        // RESUME TEMPLATE 51
        // ============================================================
        public IActionResult ViewProfileFiftyOne(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyOne");
        }


        // ============================================================
        // RESUME TEMPLATE 52
        // ============================================================
        public IActionResult ViewProfileFiftyTwo(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyTwo");
        }


        // ============================================================
        // RESUME TEMPLATE 53
        // ============================================================
        public IActionResult ViewProfileFiftyThree(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyThree");
        }


        // ============================================================
        // RESUME TEMPLATE 54
        // ============================================================
        public IActionResult ViewProfileFiftyFour(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyFour");
        }


        // ============================================================
        // RESUME TEMPLATE 55
        // ============================================================
        public IActionResult ViewProfileFiftyFive(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyFive");
        }


        // ============================================================
        // RESUME TEMPLATE 56
        // ============================================================
        public IActionResult ViewProfileFiftySix(int id)
        {
            return RenderProfile(id, "ViewProfileFiftySix");
        }


        // ============================================================
        // RESUME TEMPLATE 57
        // ============================================================
        public IActionResult ViewProfileFiftySeven(int id)
        {
            return RenderProfile(id, "ViewProfileFiftySeven");
        }


        // ============================================================
        // RESUME TEMPLATE 58
        // ============================================================
        public IActionResult ViewProfileFiftyEight(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyEight");
        }


        // ============================================================
        // RESUME TEMPLATE 59
        // ============================================================
        public IActionResult ViewProfileFiftyNine(int id)
        {
            return RenderProfile(id, "ViewProfileFiftyNine");
        }

        // ============================================================
        // RESUME TEMPLATES 91 - 100 (PORTRAIT)
        // ============================================================
        public IActionResult ViewProfileNinetyOne(int id) => RenderProfile(id, "ViewProfileNinetyOne");
        public IActionResult ViewProfileNinetyTwo(int id) => RenderProfile(id, "ViewProfileNinetyTwo");
        public IActionResult ViewProfileNinetyThree(int id) => RenderProfile(id, "ViewProfileNinetyThree");
        public IActionResult ViewProfileNinetyFour(int id) => RenderProfile(id, "ViewProfileNinetyFour");
        public IActionResult ViewProfileNinetyFive(int id) => RenderProfile(id, "ViewProfileNinetyFive");
        public IActionResult ViewProfileNinetySix(int id) => RenderProfile(id, "ViewProfileNinetySix");
        public IActionResult ViewProfileNinetySeven(int id) => RenderProfile(id, "ViewProfileNinetySeven");
        public IActionResult ViewProfileNinetyEight(int id) => RenderProfile(id, "ViewProfileNinetyEight");
        public IActionResult ViewProfileNinetyNine(int id) => RenderProfile(id, "ViewProfileNinetyNine");
        public IActionResult ViewProfileHundred(int id) => RenderProfile(id, "ViewProfileHundred");

    }
}