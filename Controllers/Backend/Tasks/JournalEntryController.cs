﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Frapid.ApplicationState.Cache;
using Frapid.Dashboard;
using Frapid.i18n;
using MixERP.Finance.DAL;
using MixERP.Finance.QueryModels;
using MixERP.Finance.ViewModels;
using Frapid.Areas.CSRF;
using MixERP.Finance.Models;

namespace MixERP.Finance.Controllers.Backend.Tasks
{
    [AntiForgery]
    public class JournalEntryController : FinanceDashboardController
    {
        [Route("dashboard/finance/tasks/journal/entry")]
        [MenuPolicy]
        public ActionResult Index()
        {
            return this.FrapidView(this.GetRazorView<AreaRegistration>("Tasks/JournalEntry/Index.cshtml", this.Tenant));
        }

        [Route("dashboard/finance/tasks/journal/checklist/{tranId}")]
        [MenuPolicy(OverridePath = "/dashboard/finance/tasks/journal/entry")]
        public ActionResult CheckList(long tranId)
        {
            return this.FrapidView(this.GetRazorView<AreaRegistration>("Tasks/JournalEntry/CheckList.cshtml", this.Tenant), tranId);
        }

        [Route("dashboard/finance/tasks/journal/view")]
        [HttpPost]
        public async Task<ActionResult> GetAsync(JournalViewQuery query)
        {
            var appUser = await AppUsers.GetCurrentAsync().ConfigureAwait(true);

            query.OfficeId = appUser.OfficeId;
            query.UserId = appUser.UserId;

            var model = await Journals.GetJournalViewAsync(this.Tenant, query).ConfigureAwait(true);
            return this.Ok(model);
        }


        [Route("dashboard/finance/tasks/journal/entry/new")]
        [MenuPolicy(OverridePath = "/dashboard/finance/tasks/journal/entry")]
        public ActionResult New()
        {
            return this.FrapidView(this.GetRazorView<AreaRegistration>("Tasks/JournalEntry/New.cshtml", this.Tenant));
        }

        [Route("dashboard/finance/tasks/journal/entry/new")]
        [HttpPost]
        public async Task<ActionResult> PostAsync(TransactionPosting model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.InvalidModelState(this.ModelState);
            }
          
            try
            {
                var meta = await AppUsers.GetCurrentAsync().ConfigureAwait(true);
                long tranId = await JournalEntries.PostAsync(this.Tenant, model, meta).ConfigureAwait(true);
                return this.Ok(tranId);
            }
            catch (Exception ex)
            {
                return this.Failed(ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}