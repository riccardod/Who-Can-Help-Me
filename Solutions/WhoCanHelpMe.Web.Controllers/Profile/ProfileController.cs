namespace WhoCanHelpMe.Web.Controllers.Profile
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Web.Mvc;

    using ActionFilters;

    using Domain;
    using Domain.Contracts.Tasks;

    using Extensions;

    using Home;

    using MvcContrib;
    using MvcContrib.Filters;

    using Shared.ActionResults;

    using ViewModels;

    using WhoCanHelpMe.Framework.Mapper;
    using WhoCanHelpMe.Web.Controllers.Profile.Mappers.Contracts;

    using xVal.ServerSide;

    #endregion

    public class ProfileController : BaseController
    {
        private readonly ICategoryTasks categoryTasks;

        private readonly IIdentityTasks identityTasks;

        private readonly ICreateProfilePageViewModelBuilder createProfilePageViewModelBuilder;

        private readonly IMapper<Profile, ViewProfilePageViewModel> viewProfilePageViewModelMapper;

        private readonly IMapper<Profile, IList<Category>, UpdateProfilePageViewModel> updateProfilePageViewModelMapper;

        private readonly IProfileTasks userTasks;

        public ProfileController(
            IIdentityTasks identityTasks,
            IProfileTasks userTasks,
            ICategoryTasks categoryTasks,
            IMapper<Profile, ViewProfilePageViewModel> viewProfilePageViewModelMapper,
            IMapper<Profile, IList<Category>, UpdateProfilePageViewModel> updateProfilePageViewModelMapper, 
            ICreateProfilePageViewModelBuilder createProfilePageViewModelBuilder)
        {
            this.identityTasks = identityTasks;
            this.createProfilePageViewModelBuilder = createProfilePageViewModelBuilder;
            this.userTasks = userTasks;
            this.categoryTasks = categoryTasks;
            this.viewProfilePageViewModelMapper = viewProfilePageViewModelMapper;
            this.updateProfilePageViewModelMapper = updateProfilePageViewModelMapper;
        }

        [Authorize]
        [HttpGet]
        [ModelStateToTempData]
        [RequireNoExistingProfile("Profile", "Update")]
        public ActionResult Create()
        {
            var viewModel = this.createProfilePageViewModelBuilder.Get();

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModelStateToTempData]
        [RequireNoExistingProfile("Profile", "Update")]
        public ActionResult Create(CreateProfileFormModel formModel)
        {
            var identity = this.identityTasks.GetCurrentIdentity();

            try
            {
                this.userTasks.CreateProfile(identity.UserName, formModel.FirstName, formModel.LastName);

                return this.RedirectToAction(x => x.Update());
            }
            catch (RulesException ex)
            {
                ex.AddModelStateErrors(this.ModelState);
            }

            return this.RedirectToAction(x => x.Create());
        }

        [Authorize]
        [HttpGet]
        [RequireExistingProfile("Profile", "Create")]
        public ActionResult Delete()
        {
            var identity = this.identityTasks.GetCurrentIdentity();

            this.userTasks.DeleteProfile(identity.UserName);

            return this.RedirectToAction<HomeController>(x => x.Index());
        }

        [Authorize]
        [HttpGet]
        [RequireExistingProfile("Profile", "Create")]
        public ActionResult DeleteAssertion(int assertionId)
        {
            var identity = this.identityTasks.GetCurrentIdentity();

            var user = this.userTasks.GetProfileByUserName(identity.UserName);

            this.userTasks.RemoveAssertion(
                user,
                assertionId);

            return this.RedirectToAction(x => x.Update());
        }

        [HttpGet]
        public ActionResult Index()
        {
            return this.RedirectToAction(x => x.Update());
        }

        [Authorize]
        [HttpGet]
        [ModelStateToTempData]
        [RequireExistingProfile("Profile", "Create")]
        public ActionResult Update()
        {
            var identity = this.identityTasks.GetCurrentIdentity();

            var user = this.userTasks.GetProfileByUserName(identity.UserName);

            var categories = this.categoryTasks.GetAll();

            var viewModel = this.updateProfilePageViewModelMapper.MapFrom(
                user,
                categories);

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModelStateToTempData]
        [RequireExistingProfile("Profile", "Create")]
        public ActionResult Update(AddAssertionFormModel formModel)
        {
            var identity = this.identityTasks.GetCurrentIdentity();

            try
            {
                this.userTasks.AddAssertion(identity.UserName, formModel.CategoryId, formModel.TagName);
            }
            catch (RulesException ex)
            {
                ex.AddModelStateErrors(
                    this.ModelState,
                    "addAssertion");
            }

            return this.RedirectToAction(x => x.Update());
        }

        [HttpGet]
        public ActionResult View(int id)
        {
            var user = this.userTasks.GetProfileById(id);

            if (user != null)
            {
                var profileViewModel = this.viewProfilePageViewModelMapper.MapFrom(user);

                return this.View(profileViewModel);
            }

            return new NotFoundResult();
        }
    }
}