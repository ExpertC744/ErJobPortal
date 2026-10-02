using ErJobPortal.Models;
using ErJobPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ErJobPortal.Controllers
{
    public class ResumeController : Controller
    {
        private readonly CandidateProfileRepository _repository;

        public ResumeController(
            CandidateProfileRepository repository)
        {
            _repository = repository;
        }




        // shrirang 30/09/26

        [HttpGet]
        public IActionResult Resume(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses(),
                MedicalSkills = _repository.GetMedicalSkills(),
                TechnicalSkills = _repository.GetTechnicalSkills(),
                NonTechnicalSkills = _repository.GetNonTechnicalSkills()
            };

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Rahul Raut";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/Resume.cshtml", model);
        }
        // end


        //public IActionResult ViewProfileOne(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return BadRequest("Invalid Candidate ID.");
        //    }

        //    var profile = _repository.GetProfile(id);

        //    if (profile == null)
        //    {
        //        return NotFound($"Candidate profile not found for CandidateID: {id}");
        //    }

        //    var model = new ResumeViewModel
        //    {
        //        CandidateID = id,
        //        Profile = profile,

        //        Relationships = _repository.GetRelationships(),
        //        Streams = _repository.GetStreams(),
        //        Divisions = _repository.GetDivisions(),
        //        InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
        //        InternshipTitles = _repository.GetInternshipTitles(),
        //        InternshipDurations = _repository.GetInternshipDurations(),
        //        InternshipStatuses = _repository.GetInternshipStatuses()
        //    };

        //    // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
        //    // they must live in a separate Candidate/User account table.
        //    // Leave as-is until you tell me that table/repository.
        //    model.CandidateName = "Candidate";
        //    model.CandidateEmail = "";
        //    model.CandidatePhone = "";

        //    return View("~/Views/Resume/ViewProfileOne.cshtml", model);
        //}
        public IActionResult ViewProfileOne(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Rahul Raut";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/ViewProfileOne.cshtml", model);
        }  
        public IActionResult ViewProfileTwo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Rahul Raut";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/ViewProfileTwo.cshtml", model);
        }
         
        public IActionResult ViewProfileThree(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Rahul Raut";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/ViewProfileThree.cshtml", model);
        }  

        //public IActionResult ViewProfileThree(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return BadRequest("Invalid Candidate ID.");
        //    }

        //    var profile = _repository.GetProfile(id);

        //    if (profile == null)
        //    {
        //        return NotFound($"Candidate profile not found for CandidateID: {id}");
        //    }

        //    var model = new ResumeViewModel
        //    {
        //        CandidateID = id,
        //        Profile = profile,

        //        Relationships = _repository.GetRelationships(),
        //        Streams = _repository.GetStreams(),
        //        Divisions = _repository.GetDivisions(),
        //        InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
        //        InternshipTitles = _repository.GetInternshipTitles(),
        //        InternshipDurations = _repository.GetInternshipDurations(),
        //        InternshipStatuses = _repository.GetInternshipStatuses()
        //    };

        //    // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
        //    // they must live in a separate Candidate/User account table.
        //    // Leave as-is until you tell me that table/repository.
        //    model.CandidateName = "Candidate";
        //    model.CandidateEmail = "";
        //    model.CandidatePhone = "";

        //    return View("~/Views/Resume/ViewProfileThree.cshtml", model);
        //}

        public IActionResult ViewProfileFour(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch real candidate Name, Email and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileFour.cshtml", model);
        }


        public IActionResult ViewProfileFive(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileFive.cshtml", model);
        }


        public IActionResult ViewProfileSix(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileSix.cshtml", model);
        }


        public IActionResult ViewProfileSeven(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileSeven.cshtml", model);
        }


        public IActionResult ViewProfileEight(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileEight.cshtml", model);
        }


        public IActionResult ViewProfileNine(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileNine.cshtml", model);
        }


        public IActionResult ViewProfileTen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTen.cshtml", model);
        }
        public IActionResult ViewProfileEleven(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileEleven.cshtml", model);
        }


        public IActionResult ViewProfileTwelve(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwelve.cshtml", model);
        }


        public IActionResult ViewProfileThirteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirteen.cshtml", model);
        }


        public IActionResult ViewProfileFourteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileFourteen.cshtml", model);
        }


        public IActionResult ViewProfileFifteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileFifteen.cshtml", model);
        }


        public IActionResult ViewProfileSixteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileSixteen.cshtml", model);
        }


        public IActionResult ViewProfileSeventeen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileSeventeen.cshtml", model);
        }


        public IActionResult ViewProfileEighteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileEighteen.cshtml", model);
        }


        public IActionResult ViewProfileNineteen(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileNineteen.cshtml", model);
        }


        public IActionResult ViewProfileTwenty(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwenty.cshtml", model);
        }


        public IActionResult ViewProfileTwentyOne(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyOne.cshtml", model);
        }


        public IActionResult ViewProfileTwentyTwo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyTwo.cshtml", model);
        }


        public IActionResult ViewProfileTwentyThree(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyThree.cshtml", model);
        }


        public IActionResult ViewProfileTwentyFour(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyFour.cshtml", model);
        }


        public IActionResult ViewProfileTwentyFive(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyFive.cshtml", model);
        }


        public IActionResult ViewProfileTwentySix(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentySix.cshtml", model);
        }


        public IActionResult ViewProfileTwentySeven(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentySeven.cshtml", model);
        }


        public IActionResult ViewProfileTwentyEight(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyEight.cshtml", model);
        }


        public IActionResult ViewProfileTwentyNine(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileTwentyNine.cshtml", model);
        }


        public IActionResult ViewProfileThirty(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // Fetch the real Name, Email, and Phone
            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirty.cshtml", model);
        }
        public IActionResult ViewProfileThirtyOne(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyOne.cshtml", model);
        }


        public IActionResult ViewProfileThirtyTwo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyTwo.cshtml", model);
        }


        public IActionResult ViewProfileThirtyThree(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyThree.cshtml", model);
        }


        public IActionResult ViewProfileThirtyFour(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyFour.cshtml", model);
        }


        public IActionResult ViewProfileThirtyFive(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyFive.cshtml", model);
        }


        public IActionResult ViewProfileThirtySix(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtySix.cshtml", model);
        }


        public IActionResult ViewProfileThirtySeven(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtySeven.cshtml", model);
        }


        public IActionResult ViewProfileThirtyEight(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyEight.cshtml", model);
        }


        public IActionResult ViewProfileThirtyNine(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileThirtyNine.cshtml", model);
        }


        public IActionResult ViewProfileForty(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound($"Candidate profile not found for CandidateID: {id}");
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,
                Profile = profile,

                Relationships = _repository.GetRelationships(),
                Streams = _repository.GetStreams(),
                Divisions = _repository.GetDivisions(),
                InternshipFellowshipType = _repository.GetInternshipFellowshipType(),
                InternshipTitles = _repository.GetInternshipTitles(),
                InternshipDurations = _repository.GetInternshipDurations(),
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            var candidateAccount = _repository.GetCandidateBasicInfo(id);

            if (candidateAccount != null)
            {
                model.CandidateName = candidateAccount.sFName + " " + candidateAccount.sLName;
                model.CandidateEmail = candidateAccount.sEmail;
                model.CandidatePhone = candidateAccount.sMobile;
            }
            else
            {
                model.CandidateName = "Candidate Name";
                model.CandidateEmail = "";
                model.CandidatePhone = "";
            }

            return View("~/Views/Resume/ViewProfileForty.cshtml", model);
        }
    }
}