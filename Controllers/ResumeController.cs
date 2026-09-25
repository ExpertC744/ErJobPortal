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
                InternshipStatuses = _repository.GetInternshipStatuses()
            };

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/Resume.cshtml", model);
        }

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
            model.CandidateName = "Candidate";
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
            model.CandidateName = "Candidate";
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
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/ViewProfileThree.cshtml", model);
        }

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

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

            // NOTE: CandidateName/Email/Phone are not in tblCandidateProfile at all —
            // they must live in a separate Candidate/User account table.
            // Leave as-is until you tell me that table/repository.
            model.CandidateName = "Candidate";
            model.CandidateEmail = "";
            model.CandidatePhone = "";

            return View("~/Views/Resume/ViewProfileSixteen.cshtml", model);
        }


       
    }
}