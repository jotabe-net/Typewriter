﻿using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Typewriter.Templates;
using Typewriter.VisualStudio.Resources;

namespace Typewriter.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [Guid(GuidList.VisualStudioExtensionPackageId)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    //[ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ExtensionPackage : Package, IDisposable
    {
        //private EditorFactory editorFactory;

        private DTE dte;
        private IVsStatusbar statusBar;
        private Log log;
        private ISolutionMonitor solutionMonitor;
        private ITemplateManager templateManager;
        private IEventQueue eventQueue;
        //private CommandManager commendManager;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.dte = GetService(typeof(DTE)) as DTE;

            if (this.dte == null)
                ErrorHandler.ThrowOnFailure(1);

            this.statusBar = GetService(typeof(SVsStatusbar)) as IVsStatusbar;

            if (this.statusBar == null)
                ErrorHandler.ThrowOnFailure(1);

            this.log = new Log(dte);
            this.eventQueue = new EventQueue(statusBar, log);
            this.solutionMonitor = new SolutionMonitor(log);
            this.templateManager = new TemplateManager(log, dte, solutionMonitor, eventQueue);
            var generationManager = new GenerationManager(log, dte, solutionMonitor, templateManager, eventQueue);
            //this.commendManager = new CommandManager(generationManager);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (this.eventQueue != null)
            {
                this.eventQueue.Dispose();
                this.eventQueue = null;
            }
        }
    }
}