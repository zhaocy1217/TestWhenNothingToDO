namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class SkillSelectControl : Singleton<SkillSelectControl>
    {
        private PoolObjHandle<ActorRoot> m_SkillTargetObj;
        private DictionaryView<uint, SkillBaseSelectTarget> registedRule = new DictionaryView<uint, SkillBaseSelectTarget>();

        public override void Init()
        {
            ClassEnumerator enumerator = new ClassEnumerator(typeof(SkillBaseSelectTargetAttribute), typeof(SkillBaseSelectTarget), typeof(SkillBaseSelectTargetAttribute).Assembly, true, false, false);
            foreach (Type type in enumerator.results)
            {
                SkillBaseSelectTarget target = (SkillBaseSelectTarget) Activator.CreateInstance(type);
                Attribute customAttribute = Attribute.GetCustomAttribute(type, typeof(SkillBaseSelectTargetAttribute));
                this.registedRule.Add((uint) (customAttribute as SkillBaseSelectTargetAttribute).TargetRule, target);
            }
        }

        public bool IsLowerHpMode()
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            return ((hostPlayer == null) || (hostPlayer.AttackTargetMode == SelectEnemyType.SelectLowHp));
        }

        public ActorRoot SelectTarget(SkillTargetRule ruleType, SkillSlot slot)
        {
            SkillBaseSelectTarget target;
            if (this.m_SkillTargetObj != 0)
            {
                return (ActorRoot) this.m_SkillTargetObj;
            }
            if (this.registedRule.TryGetValue((uint) ruleType, out target))
            {
                return target.SelectTarget(slot);
            }
            return null;
        }

        public VInt3 SelectTargetDir(SkillTargetRule ruleType, SkillSlot slot)
        {
            SkillBaseSelectTarget target;
            if (this.registedRule.TryGetValue((uint) ruleType, out target))
            {
                return target.SelectTargetDir(slot);
            }
            return slot.Actor.handle.forward;
        }

        public VInt3 SelectTargetPos(SkillTargetRule ruleType, SkillSlot slot, out bool bTarget)
        {
            SkillBaseSelectTarget target;
            bTarget = false;
            if (this.registedRule.TryGetValue((uint) ruleType, out target))
            {
                ActorRoot root = target.SelectTarget(slot);
                if (root != null)
                {
                    bTarget = true;
                    return root.location;
                }
            }
            return slot.Actor.handle.location;
        }
    }
}

