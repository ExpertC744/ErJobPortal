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




        //radhika 21-09 Trainee

        [HttpGet]
        public IActionResult Resume(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Candidate ID.");
            }

            // Get latest candidate profile data
            var profile = _repository.GetProfile(id);

            if (profile == null)
            {
                return NotFound(
                    $"Candidate profile not found for CandidateID: {id}"
                );
            }

            var model = new ResumeViewModel
            {
                CandidateID = id,

                Profile = profile,

                Relationships =
                    _repository.GetRelationships(),

                Streams =
                    _repository.GetStreams(),

                Divisions =
                    _repository.GetDivisions(),

                InternshipFellowshipType =
                    _repository.GetInternshipFellowshipType(),

                InternshipTitles =
                    _repository.GetInternshipTitles(),

                InternshipDurations =
                    _repository.GetInternshipDurations(),

                InternshipStatuses =
                    _repository.GetInternshipStatuses(),

                CandidateName = "Candidate",
                CandidateEmail = "",
                CandidatePhone = ""
            };

            return View(
                "~/Views/Resume/Resume.cshtml",
                model
            );
        }


        public IActionResult viewProfileOne(int id)
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
        public IActionResult viewProfileTwo(int id)
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
        public IActionResult viewProfileThree(int id)
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
    }
}