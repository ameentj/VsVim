﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Vim;
using GlobalSettings = Vim.GlobalSettings;
using VimCoreTest.Utils;

namespace VimCoreTest
{
    [TestFixture]
    public class GlobalSettingsTest : SettingsCommonTest
    {
        protected override IVimSettings Create()
        {
            return CreateGlobal();
        }

        private IVimGlobalSettings CreateGlobal()
        {
            return new GlobalSettings();
        }

        [Test]
        public void Sanity1()
        {
            var global = CreateGlobal();
            var all = global.AllSettings;
            Assert.IsTrue(all.Any(x => x.Name == GlobalSettings.IgnoreCaseName));
            Assert.IsTrue(all.Any(x => x.Name == GlobalSettings.ShiftWidthName));
        }

    }
}
