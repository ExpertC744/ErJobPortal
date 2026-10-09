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
        //sanidhya 08/10/26
        // ============================================================
        // MAIN RESUME
        // ============================================================
        [HttpGet]
        public IActionResult Resume(
      int id,
 int? previousCandidateId,
 int? nextCandidateId,
 string? resumeSource,
int? postID,
int? orgID)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound("Candidate profile not found.");
            }

            int resumeProfile = profile.Resume_Profile ?? 1;

            if (resumeProfile < 1 || resumeProfile > 59)
            {
                resumeProfile = 1;
            }

            string actionName = resumeProfile switch
            {
                //1 => "ViewProfileOne",
                //2 => "ViewProfileTwo",
                //3 => "ViewProfileThree",
                //4 => "ViewProfileFour",
                //5 => "ViewProfileFive",
                //6 => "ViewProfileSix",
                //7 => "ViewProfileSeven",
                //8 => "ViewProfileEight",
                //9 => "ViewProfileNine",
                //10 => "ViewProfileTen",
                //11 => "ViewProfileEleven",
                //12 => "ViewProfileTwelve",
                //13 => "ViewProfileThirteen",
                //14 => "ViewProfileFourteen",
                //15 => "ViewProfileFifteen",
                //16 => "ViewProfileSixteen",
                //17 => "ViewProfileSeventeen",
                //18 => "ViewProfileEighteen",
                //19 => "ViewProfileNineteen",
                //20 => "ViewProfileTwenty",
                //21 => "ViewProfileTwentyOne",
                //22 => "ViewProfileTwentyTwo",
                //23 => "ViewProfileTwentyThree",
                //24 => "ViewProfileTwentyFour",
                //25 => "ViewProfileTwentyFive",
                //26 => "ViewProfileTwentySix",
                //27 => "ViewProfileTwentySeven",
                //28 => "ViewProfileTwentyEight",
                //29 => "ViewProfileTwentyNine",
                //30 => "ViewProfileThirty",
                //31 => "ViewProfileThirtyOne",
                //32 => "ViewProfileThirtyTwo",
                //33 => "ViewProfileThirtyThree",
                //34 => "ViewProfileThirtyFour",
                //35 => "ViewProfileThirtyFive",
                //36 => "ViewProfileThirtySix",
                //37 => "ViewProfileThirtySeven",
                //38 => "ViewProfileThirtyEight",
                //39 => "ViewProfileThirtyNine",
                //40 => "ViewProfileForty",
                //41 => "ViewProfileFortyOne",
                //42 => "ViewProfileFortyTwo",
                //43 => "ViewProfileFortyThree",
                //44 => "ViewProfileFortyFour",
                //45 => "ViewProfileFortyFive",
                //46 => "ViewProfileFortySix",
                //47 => "ViewProfileFortySeven",
                //49 => "ViewProfileFortyNine",
                //50 => "ViewProfileFifty",
                //51 => "ViewProfileFiftyOne",
                //52 => "ViewProfileFiftyTwo",
                //53 => "ViewProfileFiftyThree",
                //54 => "ViewProfileFiftyFour",
                //55 => "ViewProfileFiftyFive",
                //56 => "ViewProfileFiftySix",
                //57 => "ViewProfileFiftySeven",
                //58 => "ViewProfileFiftyEight",
                //59 => "ViewProfileFiftyNine",
                1 => "ViewProfile1",
                2 => "ViewProfile2",
                3 => "ViewProfile3",
                4 => "ViewProfile4",
                5 => "ViewProfile5",
                6 => "ViewProfile6",
                7 => "ViewProfile7",
                8 => "ViewProfile8",
                9 => "ViewProfile9",
                10 => "ViewProfile10",
                11 => "ViewProfile11",
                12 => "ViewProfile12",
                13 => "ViewProfile13",
                14 => "ViewProfile14",
                15 => "ViewProfile15",
                16 => "ViewProfile16",
                17 => "ViewProfile17",
                18 => "ViewProfile18",
                19 => "ViewProfile19",
                20 => "ViewProfile20",
                21 => "ViewProfile21",
                22 => "ViewProfile22",
                23 => "ViewProfile23",
                24 => "ViewProfile24",
                25 => "ViewProfile25",
                26 => "ViewProfile26",
                27 => "ViewProfile27",
                28 => "ViewProfile28",
                29 => "ViewProfile29",
                30 => "ViewProfile30",
                31 => "ViewProfile31",
                32 => "ViewProfile32",
                33 => "ViewProfile33",
                34 => "ViewProfile34",
                35 => "ViewProfile35",
                36 => "ViewProfile36",
                37 => "ViewProfile37",
                38 => "ViewProfile38",
                39 => "ViewProfile39",
                40 => "ViewProfile40",
                41 => "ViewProfile41",
                42 => "ViewProfile42",
                43 => "ViewProfile43",
                44 => "ViewProfile44",
                45 => "ViewProfile45",
                46 => "ViewProfile46",
                47 => "ViewProfile47",
                48 => "ViewProfile48",
                49 => "ViewProfile49",
                50 => "ViewProfile50",
                51 => "ViewProfile51",
                52 => "ViewProfile52",
                53 => "ViewProfile53",
                54 => "ViewProfile54",
                55 => "ViewProfile55",
                56 => "ViewProfile56",
                57 => "ViewProfile57",
                58 => "ViewProfile58",
                59 => "ViewProfile59",
                60 => "ViewProfile60",
                61 => "ViewProfile61",
                62 => "ViewProfile62",
                63 => "ViewProfile63",
                64 => "ViewProfile64",
                65 => "ViewProfile65",
                66 => "ViewProfile66",
                67 => "ViewProfile67",
                68 => "ViewProfile68",
                69 => "ViewProfile69",
                70 => "ViewProfile70",
                71 => "ViewProfile71",
                72 => "ViewProfile72",
                73 => "ViewProfile73",
                74 => "ViewProfile74",
                75 => "ViewProfile75",
                76 => "ViewProfile76",
                77 => "ViewProfile77",
                78 => "ViewProfile78",
                79 => "ViewProfile79",
                80 => "ViewProfile80",
                81 => "ViewProfile81",
                82 => "ViewProfile82",
                83 => "ViewProfile83",
                84 => "ViewProfile84",
                85 => "ViewProfile85",
                86 => "ViewProfile86",
                87 => "ViewProfile87",
                88 => "ViewProfile88",
                89 => "ViewProfile89",
                90 => "ViewProfile90",
                91 => "ViewProfile91",
                92 => "ViewProfile92",
                93 => "ViewProfile93",
                94 => "ViewProfile94",
                95 => "ViewProfile95",
                96 => "ViewProfile96",
                97 => "ViewProfile97",
                98 => "ViewProfile98",
                99 => "ViewProfile99",
                100 => "ViewProfile100",

               

                _ => "ViewProfileOne"
            };

            return RedirectToAction(
    actionName,
    "Resume",
    new
    {
        id = id,
        previousCandidateId = previousCandidateId,
        nextCandidateId = nextCandidateId,
        resumeSource = resumeSource,
        postID = postID,
        orgID = orgID
    }
);
        }


        // ============================================================
        // RESUME TEMPLATE 1
        // ============================================================
        public IActionResult ViewProfile1(int id)
        {
            return RenderProfile(id, "ViewProfile1");
        }

        // ============================================================
        // RESUME TEMPLATE 2
        // ============================================================
        public IActionResult ViewProfile2(int id)
        {
            return RenderProfile(id, "ViewProfile2");
        }

        // ============================================================
        // RESUME TEMPLATE 3
        // ============================================================
        public IActionResult ViewProfile3(int id)
        {
            return RenderProfile(id, "ViewProfile3");
        }

        // ============================================================
        // RESUME TEMPLATE 4
        // ============================================================
        public IActionResult ViewProfile4(int id)
        {
            return RenderProfile(id, "ViewProfile4");
        }

        // ============================================================
        // RESUME TEMPLATE 5
        // ============================================================
        public IActionResult ViewProfile5(int id)
        {
            return RenderProfile(id, "ViewProfile5");
        }

        // ============================================================
        // RESUME TEMPLATE 6
        // ============================================================
        public IActionResult ViewProfile6(int id)
        {
            return RenderProfile(id, "ViewProfile6");
        }

        // ============================================================
        // RESUME TEMPLATE 7
        // ============================================================
        public IActionResult ViewProfile7(int id)
        {
            return RenderProfile(id, "ViewProfile7");
        }

        // ============================================================
        // RESUME TEMPLATE 8
        // ============================================================
        public IActionResult ViewProfile8(int id)
        {
            return RenderProfile(id, "ViewProfile8");
        }

        // ============================================================
        // RESUME TEMPLATE 9
        // ============================================================
        public IActionResult ViewProfile9(int id)
        {
            return RenderProfile(id, "ViewProfile9");
        }

        // ============================================================
        // RESUME TEMPLATE 10
        // ============================================================
        public IActionResult ViewProfile10(int id)
        {
            return RenderProfile(id, "ViewProfile10");
        }

        // ============================================================
        // RESUME TEMPLATE 11
        // ============================================================
        public IActionResult ViewProfile11(int id)
        {
            return RenderProfile(id, "ViewProfile11");
        }

        // ============================================================
        // RESUME TEMPLATE 12
        // ============================================================
        public IActionResult ViewProfile12(int id)
        {
            return RenderProfile(id, "ViewProfile12");
        }

        // ============================================================
        // RESUME TEMPLATE 13
        // ============================================================
        public IActionResult ViewProfile13(int id)
        {
            return RenderProfile(id, "ViewProfile13");
        }

        // ============================================================
        // RESUME TEMPLATE 14
        // ============================================================
        public IActionResult ViewProfile14(int id)
        {
            return RenderProfile(id, "ViewProfile14");
        }

        // ============================================================
        // RESUME TEMPLATE 15
        // ============================================================
        public IActionResult ViewProfile15(int id)
        {
            return RenderProfile(id, "ViewProfile15");
        }

        // ============================================================
        // RESUME TEMPLATE 16
        // ============================================================
        public IActionResult ViewProfile16(int id)
        {
            return RenderProfile(id, "ViewProfile16");
        }

        // ============================================================
        // RESUME TEMPLATE 17
        // ============================================================
        public IActionResult ViewProfile17(int id)
        {
            return RenderProfile(id, "ViewProfile17");
        }

        // ============================================================
        // RESUME TEMPLATE 18
        // ============================================================
        public IActionResult ViewProfile18(int id)
        {
            return RenderProfile(id, "ViewProfile18");
        }

        // ============================================================
        // RESUME TEMPLATE 19
        // ============================================================
        public IActionResult ViewProfile19(int id)
        {
            return RenderProfile(id, "ViewProfile19");
        }

        // ============================================================
        // RESUME TEMPLATE 20
        // ============================================================
        public IActionResult ViewProfile20(int id)
        {
            return RenderProfile(id, "ViewProfile20");
        }

        // ============================================================
        // RESUME TEMPLATE 21
        // ============================================================
        public IActionResult ViewProfile21(int id)
        {
            return RenderProfile(id, "ViewProfile21");
        }

        // ============================================================
        // RESUME TEMPLATE 22
        // ============================================================
        public IActionResult ViewProfile22(int id)
        {
            return RenderProfile(id, "ViewProfile22");
        }

        // ============================================================
        // RESUME TEMPLATE 23
        // ============================================================
        public IActionResult ViewProfile23(int id)
        {
            return RenderProfile(id, "ViewProfile23");
        }

        // ============================================================
        // RESUME TEMPLATE 24
        // ============================================================
        public IActionResult ViewProfile24(int id)
        {
            return RenderProfile(id, "ViewProfile24");
        }

        // ============================================================
        // RESUME TEMPLATE 25
        // ============================================================
        public IActionResult ViewProfile25(int id)
        {
            return RenderProfile(id, "ViewProfile25");
        }

        // ============================================================
        // RESUME TEMPLATE 26
        // ============================================================
        public IActionResult ViewProfile26(int id)
        {
            return RenderProfile(id, "ViewProfile26");
        }

        // ============================================================
        // RESUME TEMPLATE 27
        // ============================================================
        public IActionResult ViewProfile27(int id)
        {
            return RenderProfile(id, "ViewProfile27");
        }

        // ============================================================
        // RESUME TEMPLATE 28
        // ============================================================
        public IActionResult ViewProfile28(int id)
        {
            return RenderProfile(id, "ViewProfile28");
        }

        // ============================================================
        // RESUME TEMPLATE 29
        // ============================================================
        public IActionResult ViewProfile29(int id)
        {
            return RenderProfile(id, "ViewProfile29");
        }

        // ============================================================
        // RESUME TEMPLATE 30
        // ============================================================
        public IActionResult ViewProfile30(int id)
        {
            return RenderProfile(id, "ViewProfile30");
        }

        // ============================================================
        // RESUME TEMPLATE 31
        // ============================================================
        public IActionResult ViewProfile31(int id)
        {
            return RenderProfile(id, "ViewProfile31");
        }

        // ============================================================
        // RESUME TEMPLATE 32
        // ============================================================
        public IActionResult ViewProfile32(int id)
        {
            return RenderProfile(id, "ViewProfile32");
        }

        // ============================================================
        // RESUME TEMPLATE 33
        // ============================================================
        public IActionResult ViewProfile33(int id)
        {
            return RenderProfile(id, "ViewProfile33");
        }

        // ============================================================
        // RESUME TEMPLATE 34
        // ============================================================
        public IActionResult ViewProfile34(int id)
        {
            return RenderProfile(id, "ViewProfile34");
        }

        // ============================================================
        // RESUME TEMPLATE 35
        // ============================================================
        public IActionResult ViewProfile35(int id)
        {
            return RenderProfile(id, "ViewProfile35");
        }

        // ============================================================
        // RESUME TEMPLATE 36
        // ============================================================
        public IActionResult ViewProfile36(int id)
        {
            return RenderProfile(id, "ViewProfile36");
        }

        // ============================================================
        // RESUME TEMPLATE 37
        // ============================================================
        public IActionResult ViewProfile37(int id)
        {
            return RenderProfile(id, "ViewProfile37");
        }

        // ============================================================
        // RESUME TEMPLATE 38
        // ============================================================
        public IActionResult ViewProfile38(int id)
        {
            return RenderProfile(id, "ViewProfile38");
        }

        // ============================================================
        // RESUME TEMPLATE 39
        // ============================================================
        public IActionResult ViewProfile39(int id)
        {
            return RenderProfile(id, "ViewProfile39");
        }

        // ============================================================
        // RESUME TEMPLATE 40
        // ============================================================
        public IActionResult ViewProfile40(int id)
        {
            return RenderProfile(id, "ViewProfile40");
        }

        // ============================================================
        // RESUME TEMPLATE 41
        // ============================================================
        public IActionResult ViewProfile41(int id)
        {
            return RenderProfile(id, "ViewProfile41");
        }

        // ============================================================
        // RESUME TEMPLATE 42
        // ============================================================
        public IActionResult ViewProfile42(int id)
        {
            return RenderProfile(id, "ViewProfile42");
        }

        // ============================================================
        // RESUME TEMPLATE 43
        // ============================================================
        public IActionResult ViewProfile43(int id)
        {
            return RenderProfile(id, "ViewProfile43");
        }

        // ============================================================
        // RESUME TEMPLATE 44
        // ============================================================
        public IActionResult ViewProfile44(int id)
        {
            return RenderProfile(id, "ViewProfile44");
        }

        // ============================================================
        // RESUME TEMPLATE 45
        // ============================================================
        public IActionResult ViewProfile45(int id)
        {
            return RenderProfile(id, "ViewProfile45");
        }

        // ============================================================
        // RESUME TEMPLATE 46
        // ============================================================
        public IActionResult ViewProfile46(int id)
        {
            return RenderProfile(id, "ViewProfile46");
        }

        // ============================================================
        // RESUME TEMPLATE 47
        // ============================================================
        public IActionResult ViewProfile47(int id)
        {
            return RenderProfile(id, "ViewProfile47");
        }

        // ============================================================
        // RESUME TEMPLATE 48
        // ============================================================
        public IActionResult ViewProfile48(int id)
        {
            return RenderProfile(id, "ViewProfile48");
        }

        // ============================================================
        // RESUME TEMPLATE 49
        // ============================================================
        public IActionResult ViewProfile49(int id)
        {
            return RenderProfile(id, "ViewProfile49");
        }

        // ============================================================
        // RESUME TEMPLATE 50
        // ============================================================
        public IActionResult ViewProfile50(int id)
        {
            return RenderProfile(id, "ViewProfile50");
        }

        // ============================================================
        // RESUME TEMPLATE 51
        // ============================================================
        public IActionResult ViewProfile51(int id)
        {
            return RenderProfile(id, "ViewProfile51");
        }

        // ============================================================
        // RESUME TEMPLATE 52
        // ============================================================
        public IActionResult ViewProfile52(int id)
        {
            return RenderProfile(id, "ViewProfile52");
        }

        // ============================================================
        // RESUME TEMPLATE 53
        // ============================================================
        public IActionResult ViewProfile53(int id)
        {
            return RenderProfile(id, "ViewProfile53");
        }

        // ============================================================
        // RESUME TEMPLATE 54
        // ============================================================
        public IActionResult ViewProfile54(int id)
        {
            return RenderProfile(id, "ViewProfile54");
        }

        // ============================================================
        // RESUME TEMPLATE 55
        // ============================================================
        public IActionResult ViewProfile55(int id)
        {
            return RenderProfile(id, "ViewProfile55");
        }

        // ============================================================
        // RESUME TEMPLATE 56
        // ============================================================
        public IActionResult ViewProfile56(int id)
        {
            return RenderProfile(id, "ViewProfile56");
        }

        // ============================================================
        // RESUME TEMPLATE 57
        // ============================================================
        public IActionResult ViewProfile57(int id)
        {
            return RenderProfile(id, "ViewProfile57");
        }

        // ============================================================
        // RESUME TEMPLATE 58
        // ============================================================
        public IActionResult ViewProfile58(int id)
        {
            return RenderProfile(id, "ViewProfile58");
        }

        // ============================================================
        // RESUME TEMPLATE 59
        // ============================================================
        public IActionResult ViewProfile59(int id)
        {
            return RenderProfile(id, "ViewProfile59");
        }

        // ============================================================
        // RESUME TEMPLATE 60
        // ============================================================
        public IActionResult ViewProfile60(int id)
        {
            return RenderProfile(id, "ViewProfile60");
        }

        // ============================================================
        // RESUME TEMPLATE 61
        // ============================================================
        public IActionResult ViewProfile61(int id)
        {
            return RenderProfile(id, "ViewProfile61");
        }

        // ============================================================
        // RESUME TEMPLATE 62
        // ============================================================
        public IActionResult ViewProfile62(int id)
        {
            return RenderProfile(id, "ViewProfile62");
        }

        // ============================================================
        // RESUME TEMPLATE 63
        // ============================================================
        public IActionResult ViewProfile63(int id)
        {
            return RenderProfile(id, "ViewProfile63");
        }

        // ============================================================
        // RESUME TEMPLATE 64
        // ============================================================
        public IActionResult ViewProfile64(int id)
        {
            return RenderProfile(id, "ViewProfile64");
        }

        // ============================================================
        // RESUME TEMPLATE 65
        // ============================================================
        public IActionResult ViewProfile65(int id)
        {
            return RenderProfile(id, "ViewProfile65");
        }

        // ============================================================
        // RESUME TEMPLATE 66
        // ============================================================
        public IActionResult ViewProfile66(int id)
        {
            return RenderProfile(id, "ViewProfile66");
        }

        // ============================================================
        // RESUME TEMPLATE 67
        // ============================================================
        public IActionResult ViewProfile67(int id)
        {
            return RenderProfile(id, "ViewProfile67");
        }

        // ============================================================
        // RESUME TEMPLATE 68
        // ============================================================
        public IActionResult ViewProfile68(int id)
        {
            return RenderProfile(id, "ViewProfile68");
        }

        // ============================================================
        // RESUME TEMPLATE 69
        // ============================================================
        public IActionResult ViewProfile69(int id)
        {
            return RenderProfile(id, "ViewProfile69");
        }

        // ============================================================
        // RESUME TEMPLATE 70
        // ============================================================
        public IActionResult ViewProfile70(int id)
        {
            return RenderProfile(id, "ViewProfile70");
        }

        // ============================================================
        // RESUME TEMPLATE 71
        // ============================================================
        public IActionResult ViewProfile71(int id)
        {
            return RenderProfile(id, "ViewProfile71");
        }

        // ============================================================
        // RESUME TEMPLATE 72
        // ============================================================
        public IActionResult ViewProfile72(int id)
        {
            return RenderProfile(id, "ViewProfile72");
        }

        // ============================================================
        // RESUME TEMPLATE 73
        // ============================================================
        public IActionResult ViewProfile73(int id)
        {
            return RenderProfile(id, "ViewProfile73");
        }

        // ============================================================
        // RESUME TEMPLATE 74
        // ============================================================
        public IActionResult ViewProfile74(int id)
        {
            return RenderProfile(id, "ViewProfile74");
        }

        // ============================================================
        // RESUME TEMPLATE 75
        // ============================================================
        public IActionResult ViewProfile75(int id)
        {
            return RenderProfile(id, "ViewProfile75");
        }

        // ============================================================
        // RESUME TEMPLATE 76
        // ============================================================
        public IActionResult ViewProfile76(int id)
        {
            return RenderProfile(id, "ViewProfile76");
        }

        // ============================================================
        // RESUME TEMPLATE 77
        // ============================================================
        public IActionResult ViewProfile77(int id)
        {
            return RenderProfile(id, "ViewProfile77");
        }

        // ============================================================
        // RESUME TEMPLATE 78
        // ============================================================
        public IActionResult ViewProfile78(int id)
        {
            return RenderProfile(id, "ViewProfile78");
        }

        // ============================================================
        // RESUME TEMPLATE 79
        // ============================================================
        public IActionResult ViewProfile79(int id)
        {
            return RenderProfile(id, "ViewProfile79");
        }

        // ============================================================
        // RESUME TEMPLATE 80
        // ============================================================
        public IActionResult ViewProfile80(int id)
        {
            return RenderProfile(id, "ViewProfile80");
        }

        // ============================================================
        // RESUME TEMPLATE 81
        // ============================================================
        public IActionResult ViewProfile81(int id)
        {
            return RenderProfile(id, "ViewProfile81");
        }

        // ============================================================
        // RESUME TEMPLATE 82
        // ============================================================
        public IActionResult ViewProfile82(int id)
        {
            return RenderProfile(id, "ViewProfile82");
        }

        // ============================================================
        // RESUME TEMPLATE 83
        // ============================================================
        public IActionResult ViewProfile83(int id)
        {
            return RenderProfile(id, "ViewProfile83");
        }

        // ============================================================
        // RESUME TEMPLATE 84
        // ============================================================
        public IActionResult ViewProfile84(int id)
        {
            return RenderProfile(id, "ViewProfile84");
        }

        // ============================================================
        // RESUME TEMPLATE 85
        // ============================================================
        public IActionResult ViewProfile85(int id)
        {
            return RenderProfile(id, "ViewProfile85");
        }

        // ============================================================
        // RESUME TEMPLATE 86
        // ============================================================
        public IActionResult ViewProfile86(int id)
        {
            return RenderProfile(id, "ViewProfile86");
        }

        // ============================================================
        // RESUME TEMPLATE 87
        // ============================================================
        public IActionResult ViewProfile87(int id)
        {
            return RenderProfile(id, "ViewProfile87");
        }

        // ============================================================
        // RESUME TEMPLATE 88
        // ============================================================
        public IActionResult ViewProfile88(int id)
        {
            return RenderProfile(id, "ViewProfile88");
        }

        // ============================================================
        // RESUME TEMPLATE 89
        // ============================================================
        public IActionResult ViewProfile89(int id)
        {
            return RenderProfile(id, "ViewProfile89");
        }

        // ============================================================
        // RESUME TEMPLATE 90
        // ============================================================
        public IActionResult ViewProfile90(int id)
        {
            return RenderProfile(id, "ViewProfile90");
        }

        // ============================================================
        // RESUME TEMPLATE 91
        // ============================================================
        public IActionResult ViewProfile91(int id)
        {
            return RenderProfile(id, "ViewProfile91");
        }

        // ============================================================
        // RESUME TEMPLATE 92
        // ============================================================
        public IActionResult ViewProfile92(int id)
        {
            return RenderProfile(id, "ViewProfile92");
        }

        // ============================================================
        // RESUME TEMPLATE 93
        // ============================================================
        public IActionResult ViewProfile93(int id)
        {
            return RenderProfile(id, "ViewProfile93");
        }

        // ============================================================
        // RESUME TEMPLATE 94
        // ============================================================
        public IActionResult ViewProfile94(int id)
        {
            return RenderProfile(id, "ViewProfile94");
        }

        // ============================================================
        // RESUME TEMPLATE 95
        // ============================================================
        public IActionResult ViewProfile95(int id)
        {
            return RenderProfile(id, "ViewProfile95");
        }

        // ============================================================
        // RESUME TEMPLATE 96
        // ============================================================
        public IActionResult ViewProfile96(int id)
        {
            return RenderProfile(id, "ViewProfile96");
        }

        // ============================================================
        // RESUME TEMPLATE 97
        // ============================================================
        public IActionResult ViewProfile97(int id)
        {
            return RenderProfile(id, "ViewProfile97");
        }

        // ============================================================
        // RESUME TEMPLATE 98
        // ============================================================
        public IActionResult ViewProfile98(int id)
        {
            return RenderProfile(id, "ViewProfile98");
        }

        // ============================================================
        // RESUME TEMPLATE 99
        // ============================================================
        public IActionResult ViewProfile99(int id)
        {
            return RenderProfile(id, "ViewProfile99");
        }

        // ============================================================
        // RESUME TEMPLATE 100
        // ============================================================
        public IActionResult ViewProfile100(int id)
        {
            return RenderProfile(id, "ViewProfile100");
        }

    }
}