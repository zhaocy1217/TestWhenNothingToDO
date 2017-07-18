namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public class HeroViewSortImp : Singleton<HeroViewSortImp>, IMallSort<CMallSortHelper.HeroViewSortType>, IComparer<IHeroData>
    {
        private CultureInfo m_culture;
        private bool m_desc;
        private CMallSortHelper.HeroViewSortType m_sortType;

        public int Compare(IHeroData l, IHeroData r)
        {
            if ((l == null) || (l.heroCfgInfo == null))
            {
                return 1;
            }
            if ((r == null) || (r.heroCfgInfo == null))
            {
                return -1;
            }
            switch (this.m_sortType)
            {
                case CMallSortHelper.HeroViewSortType.Name:
                    return this.CompareName(l, r);

                case CMallSortHelper.HeroViewSortType.Proficiency:
                    return this.CompareProficiency(l, r);

                case CMallSortHelper.HeroViewSortType.ReleaseTime:
                    return this.CompareReleaseTime(l, r);
            }
            return this.CompareDefault(l, r);
        }

        private int CompareDefault(IHeroData l, IHeroData r)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "CompareDefault role is null");
                return 0;
            }
            if (l.bPlayerOwn && !r.bPlayerOwn)
            {
                return -1;
            }
            if (r.bPlayerOwn && !l.bPlayerOwn)
            {
                return 1;
            }
            if (!r.bPlayerOwn && !l.bPlayerOwn)
            {
                if (masterRoleInfo.IsFreeHero(l.cfgID) && !masterRoleInfo.IsFreeHero(r.cfgID))
                {
                    return -1;
                }
                if (masterRoleInfo.IsFreeHero(r.cfgID) && !masterRoleInfo.IsFreeHero(l.cfgID))
                {
                    return 1;
                }
                if (!masterRoleInfo.IsFreeHero(r.cfgID) && !masterRoleInfo.IsFreeHero(l.cfgID))
                {
                    if (masterRoleInfo.IsValidExperienceHero(l.cfgID) && !masterRoleInfo.IsValidExperienceHero(r.cfgID))
                    {
                        return -1;
                    }
                    if (masterRoleInfo.IsValidExperienceHero(r.cfgID) && !masterRoleInfo.IsValidExperienceHero(l.cfgID))
                    {
                        return 1;
                    }
                }
            }
            return l.sortId.CompareTo(r.sortId);
        }

        private int CompareName(IHeroData l, IHeroData r)
        {
            return string.Compare(l.heroCfgInfo.szName, r.heroCfgInfo.szName, this.m_culture, CompareOptions.None);
        }

        private int CompareProficiency(IHeroData l, IHeroData r)
        {
            if (l.proficiencyLV != r.proficiencyLV)
            {
                return r.proficiencyLV.CompareTo(l.proficiencyLV);
            }
            if (l.proficiency != r.proficiency)
            {
                return r.proficiency.CompareTo(l.proficiency);
            }
            if (l.bPlayerOwn && !r.bPlayerOwn)
            {
                return -1;
            }
            if (r.bPlayerOwn && !l.bPlayerOwn)
            {
                return 1;
            }
            return this.CompareName(l, r);
        }

        private int CompareReleaseTime(IHeroData l, IHeroData r)
        {
            ResHeroShop shop = null;
            ResHeroShop shop2 = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(l.cfgID, out shop);
            GameDataMgr.heroShopInfoDict.TryGetValue(r.cfgID, out shop2);
            if (shop == null)
            {
                return 1;
            }
            if (shop2 == null)
            {
                return -1;
            }
            return shop.dwReleaseId.CompareTo(shop2.dwReleaseId);
        }

        public CMallSortHelper.HeroViewSortType GetCurSortType()
        {
            return this.m_sortType;
        }

        public string GetSortTypeName(CMallSortHelper.HeroViewSortType sortType = 0)
        {
            int num = (int) sortType;
            if ((num >= 0) && (num <= CMallSortHelper.heroViewSortTypeNameKeys.Length))
            {
                return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.heroViewSortTypeNameKeys[(int) sortType]);
            }
            return null;
        }

        public override void Init()
        {
            base.Init();
            this.m_sortType = CMallSortHelper.HeroViewSortType.Default;
            this.m_desc = false;
            this.m_culture = new CultureInfo("zh-CN");
        }

        public bool IsDesc()
        {
            return this.m_desc;
        }

        public void SetDesc(bool bDesc)
        {
            this.m_desc = bDesc;
        }

        public void SetSortType(CMallSortHelper.HeroViewSortType sortType = 0)
        {
            this.m_sortType = sortType;
            if (this.m_sortType == CMallSortHelper.HeroViewSortType.Default)
            {
                this.m_desc = false;
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_sortType = CMallSortHelper.HeroViewSortType.Default;
            this.m_desc = false;
            this.m_culture = null;
        }
    }
}

