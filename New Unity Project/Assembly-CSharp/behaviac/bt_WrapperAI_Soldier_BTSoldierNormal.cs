﻿namespace behaviac
{
    using System;

    public static class bt_WrapperAI_Soldier_BTSoldierNormal
    {
        public static bool build_behavior_tree(BehaviorTree bt)
        {
            bt.SetClassNameString("BehaviorTree");
            bt.SetId(-1);
            bt.SetName("WrapperAI/Soldier/BTSoldierNormal");
            bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
            bt.AddPar("uint", "p_targetID", "0", string.Empty);
            bt.AddPar("int", "p_srchRange", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
            bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
            bt.AddPar("uint", "p_CmdID", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_attackPathCurTargetPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("int", "p_pursueTime", "0", string.Empty);
            bt.AddPar("uint", "p_abandonTargetID", "0", string.Empty);
            bt.AddPar("uint", "p_tempTargetId", "0", string.Empty);
            bt.AddPar("ushort", "p_chooseTargetCount", "0", string.Empty);
            Sequence pChild = new Sequence();
            pChild.SetClassNameString("Sequence");
            pChild.SetId(0);
            bt.AddChild(pChild);
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node106 _node = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node106();
            _node.SetClassNameString("Assignment");
            _node.SetId(0x6a);
            pChild.AddChild(_node);
            pChild.SetHasEvents(pChild.HasEvents() | _node.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node14 _node2 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node14();
            _node2.SetClassNameString("DecoratorLoop");
            _node2.SetId(14);
            pChild.AddChild(_node2);
            SelectorLoop loop = new SelectorLoop();
            loop.SetClassNameString("SelectorLoop");
            loop.SetId(1);
            _node2.AddChild(loop);
            WithPrecondition precondition = new WithPrecondition();
            precondition.SetClassNameString("WithPrecondition");
            precondition.SetId(0x3b);
            loop.AddChild(precondition);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node78 _node3 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node78();
            _node3.SetClassNameString("Condition");
            _node3.SetId(0x4e);
            precondition.AddChild(_node3);
            precondition.SetHasEvents(precondition.HasEvents() | _node3.HasEvents());
            Sequence sequence2 = new Sequence();
            sequence2.SetClassNameString("Sequence");
            sequence2.SetId(0x69);
            precondition.AddChild(sequence2);
            IfElse @else = new IfElse();
            @else.SetClassNameString("IfElse");
            @else.SetId(10);
            sequence2.AddChild(@else);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node11 _node4 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node11();
            _node4.SetClassNameString("Condition");
            _node4.SetId(11);
            @else.AddChild(_node4);
            @else.SetHasEvents(@else.HasEvents() | _node4.HasEvents());
            Noop noop = new Noop();
            noop.SetClassNameString("Noop");
            noop.SetId(12);
            @else.AddChild(noop);
            @else.SetHasEvents(@else.HasEvents() | noop.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node107 _node5 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node107();
            _node5.SetClassNameString("Assignment");
            _node5.SetId(0x6b);
            @else.AddChild(_node5);
            @else.SetHasEvents(@else.HasEvents() | _node5.HasEvents());
            sequence2.SetHasEvents(sequence2.HasEvents() | @else.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node26 _node6 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node26();
            _node6.SetClassNameString("Assignment");
            _node6.SetId(0x1a);
            sequence2.AddChild(_node6);
            sequence2.SetHasEvents(sequence2.HasEvents() | _node6.HasEvents());
            Parallel_bt_WrapperAI_Soldier_BTSoldierNormal_node2 _node7 = new Parallel_bt_WrapperAI_Soldier_BTSoldierNormal_node2();
            _node7.SetClassNameString("Parallel");
            _node7.SetId(2);
            sequence2.AddChild(_node7);
            Sequence sequence3 = new Sequence();
            sequence3.SetClassNameString("Sequence");
            sequence3.SetId(3);
            _node7.AddChild(sequence3);
            IfElse else2 = new IfElse();
            else2.SetClassNameString("IfElse");
            else2.SetId(0x24);
            sequence3.AddChild(else2);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node4 _node8 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node4();
            _node8.SetClassNameString("Condition");
            _node8.SetId(4);
            else2.AddChild(_node8);
            else2.SetHasEvents(else2.HasEvents() | _node8.HasEvents());
            True @true = new True();
            @true.SetClassNameString("True");
            @true.SetId(0x27);
            else2.AddChild(@true);
            else2.SetHasEvents(else2.HasEvents() | @true.HasEvents());
            DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierNormal_node37 _node9 = new DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierNormal_node37();
            _node9.SetClassNameString("DecoratorAlwaysFailure");
            _node9.SetId(0x25);
            else2.AddChild(_node9);
            Sequence sequence4 = new Sequence();
            sequence4.SetClassNameString("Sequence");
            sequence4.SetId(0x29);
            _node9.AddChild(sequence4);
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node38 _node10 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node38();
            _node10.SetClassNameString("Assignment");
            _node10.SetId(0x26);
            sequence4.AddChild(_node10);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node10.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node42 _node11 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node42();
            _node11.SetClassNameString("Assignment");
            _node11.SetId(0x2a);
            sequence4.AddChild(_node11);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node11.HasEvents());
            _node9.SetHasEvents(_node9.HasEvents() | sequence4.HasEvents());
            else2.SetHasEvents(else2.HasEvents() | _node9.HasEvents());
            sequence3.SetHasEvents(sequence3.HasEvents() | else2.HasEvents());
            Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node20 _node12 = new Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node20();
            _node12.SetClassNameString("Compute");
            _node12.SetId(20);
            sequence3.AddChild(_node12);
            sequence3.SetHasEvents(sequence3.HasEvents() | _node12.HasEvents());
            _node7.SetHasEvents(_node7.HasEvents() | sequence3.HasEvents());
            SelectorLoop loop2 = new SelectorLoop();
            loop2.SetClassNameString("SelectorLoop");
            loop2.SetId(0x34);
            _node7.AddChild(loop2);
            WithPrecondition precondition2 = new WithPrecondition();
            precondition2.SetClassNameString("WithPrecondition");
            precondition2.SetId(0x37);
            loop2.AddChild(precondition2);
            Sequence sequence5 = new Sequence();
            sequence5.SetClassNameString("Sequence");
            sequence5.SetId(0x38);
            precondition2.AddChild(sequence5);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node63 _node13 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node63();
            _node13.SetClassNameString("Condition");
            _node13.SetId(0x3f);
            sequence5.AddChild(_node13);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node13.HasEvents());
            Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node68 _node14 = new Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node68();
            _node14.SetClassNameString("Compute");
            _node14.SetId(0x44);
            sequence5.AddChild(_node14);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node14.HasEvents());
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node64 _node15 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node64();
            _node15.SetClassNameString("Condition");
            _node15.SetId(0x40);
            sequence5.AddChild(_node15);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node15.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node61 _node16 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node61();
            _node16.SetClassNameString("Assignment");
            _node16.SetId(0x3d);
            sequence5.AddChild(_node16);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node16.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node67 _node17 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node67();
            _node17.SetClassNameString("Assignment");
            _node17.SetId(0x43);
            sequence5.AddChild(_node17);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node17.HasEvents());
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node58 _node18 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node58();
            _node18.SetClassNameString("Condition");
            _node18.SetId(0x3a);
            sequence5.AddChild(_node18);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node18.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node60 _node19 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node60();
            _node19.SetClassNameString("Assignment");
            _node19.SetId(60);
            sequence5.AddChild(_node19);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node19.HasEvents());
            precondition2.SetHasEvents(precondition2.HasEvents() | sequence5.HasEvents());
            False @false = new False();
            @false.SetClassNameString("False");
            @false.SetId(0x39);
            precondition2.AddChild(@false);
            precondition2.SetHasEvents(precondition2.HasEvents() | @false.HasEvents());
            loop2.SetHasEvents(loop2.HasEvents() | precondition2.HasEvents());
            WithPrecondition precondition3 = new WithPrecondition();
            precondition3.SetClassNameString("WithPrecondition");
            precondition3.SetId(5);
            loop2.AddChild(precondition3);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node109 _node20 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node109();
            _node20.SetClassNameString("Condition");
            _node20.SetId(0x6d);
            precondition3.AddChild(_node20);
            precondition3.SetHasEvents(precondition3.HasEvents() | _node20.HasEvents());
            Sequence sequence6 = new Sequence();
            sequence6.SetClassNameString("Sequence");
            sequence6.SetId(110);
            precondition3.AddChild(sequence6);
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node40 _node21 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node40();
            _node21.SetClassNameString("Assignment");
            _node21.SetId(40);
            sequence6.AddChild(_node21);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node21.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node111 _node22 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node111();
            _node22.SetClassNameString("Action");
            _node22.SetId(0x6f);
            sequence6.AddChild(_node22);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node22.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node112 _node23 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node112();
            _node23.SetClassNameString("Action");
            _node23.SetId(0x70);
            sequence6.AddChild(_node23);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node23.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node44 _node24 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node44();
            _node24.SetClassNameString("Assignment");
            _node24.SetId(0x2c);
            sequence6.AddChild(_node24);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node24.HasEvents());
            IfElse else3 = new IfElse();
            else3.SetClassNameString("IfElse");
            else3.SetId(0x30);
            sequence6.AddChild(else3);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node6 _node25 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node6();
            _node25.SetClassNameString("Condition");
            _node25.SetId(6);
            else3.AddChild(_node25);
            else3.SetHasEvents(else3.HasEvents() | _node25.HasEvents());
            Sequence sequence7 = new Sequence();
            sequence7.SetClassNameString("Sequence");
            sequence7.SetId(7);
            else3.AddChild(sequence7);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node23 _node26 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node23();
            _node26.SetClassNameString("Action");
            _node26.SetId(0x17);
            sequence7.AddChild(_node26);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node26.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node113 _node27 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node113();
            _node27.SetClassNameString("Action");
            _node27.SetId(0x71);
            sequence7.AddChild(_node27);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node27.HasEvents());
            Selector selector = new Selector();
            selector.SetClassNameString("Selector");
            selector.SetId(0x11);
            sequence7.AddChild(selector);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node114 _node28 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node114();
            _node28.SetClassNameString("Action");
            _node28.SetId(0x72);
            selector.AddChild(_node28);
            selector.SetHasEvents(selector.HasEvents() | _node28.HasEvents());
            DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierNormal_node19 _node29 = new DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierNormal_node19();
            _node29.SetClassNameString("DecoratorAlwaysFailure");
            _node29.SetId(0x13);
            selector.AddChild(_node29);
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node18 _node30 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node18();
            _node30.SetClassNameString("Assignment");
            _node30.SetId(0x12);
            _node29.AddChild(_node30);
            _node29.SetHasEvents(_node29.HasEvents() | _node30.HasEvents());
            selector.SetHasEvents(selector.HasEvents() | _node29.HasEvents());
            sequence7.SetHasEvents(sequence7.HasEvents() | selector.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node140 _node31 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node140();
            _node31.SetClassNameString("Action");
            _node31.SetId(140);
            sequence7.AddChild(_node31);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node31.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node116 _node32 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node116();
            _node32.SetClassNameString("Action");
            _node32.SetId(0x74);
            sequence7.AddChild(_node32);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node32.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node117 _node33 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node117();
            _node33.SetClassNameString("DecoratorLoopUntil");
            _node33.SetId(0x75);
            sequence7.AddChild(_node33);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node118 _node34 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node118();
            _node34.SetClassNameString("Condition");
            _node34.SetId(0x76);
            _node33.AddChild(_node34);
            _node33.SetHasEvents(_node33.HasEvents() | _node34.HasEvents());
            sequence7.SetHasEvents(sequence7.HasEvents() | _node33.HasEvents());
            else3.SetHasEvents(else3.HasEvents() | sequence7.HasEvents());
            Sequence sequence8 = new Sequence();
            sequence8.SetClassNameString("Sequence");
            sequence8.SetId(9);
            else3.AddChild(sequence8);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node8 _node35 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node8();
            _node35.SetClassNameString("Action");
            _node35.SetId(8);
            sequence8.AddChild(_node35);
            sequence8.SetHasEvents(sequence8.HasEvents() | _node35.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node13 _node36 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node13();
            _node36.SetClassNameString("Action");
            _node36.SetId(13);
            sequence8.AddChild(_node36);
            sequence8.SetHasEvents(sequence8.HasEvents() | _node36.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node15 _node37 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node15();
            _node37.SetClassNameString("DecoratorLoopUntil");
            _node37.SetId(15);
            sequence8.AddChild(_node37);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node16 _node38 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node16();
            _node38.SetClassNameString("Condition");
            _node38.SetId(0x10);
            _node37.AddChild(_node38);
            _node37.SetHasEvents(_node37.HasEvents() | _node38.HasEvents());
            sequence8.SetHasEvents(sequence8.HasEvents() | _node37.HasEvents());
            else3.SetHasEvents(else3.HasEvents() | sequence8.HasEvents());
            sequence6.SetHasEvents(sequence6.HasEvents() | else3.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node28 _node39 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node28();
            _node39.SetClassNameString("Action");
            _node39.SetId(0x1c);
            sequence6.AddChild(_node39);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node39.HasEvents());
            precondition3.SetHasEvents(precondition3.HasEvents() | sequence6.HasEvents());
            loop2.SetHasEvents(loop2.HasEvents() | precondition3.HasEvents());
            WithPrecondition precondition4 = new WithPrecondition();
            precondition4.SetClassNameString("WithPrecondition");
            precondition4.SetId(0x35);
            loop2.AddChild(precondition4);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node54 _node40 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node54();
            _node40.SetClassNameString("Condition");
            _node40.SetId(0x36);
            precondition4.AddChild(_node40);
            precondition4.SetHasEvents(precondition4.HasEvents() | _node40.HasEvents());
            Sequence sequence9 = new Sequence();
            sequence9.SetClassNameString("Sequence");
            sequence9.SetId(0x80);
            precondition4.AddChild(sequence9);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node129 _node41 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node129();
            _node41.SetClassNameString("Action");
            _node41.SetId(0x81);
            sequence9.AddChild(_node41);
            sequence9.SetHasEvents(sequence9.HasEvents() | _node41.HasEvents());
            IfElse else4 = new IfElse();
            else4.SetClassNameString("IfElse");
            else4.SetId(0x77);
            sequence9.AddChild(else4);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node120 _node42 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node120();
            _node42.SetClassNameString("Condition");
            _node42.SetId(120);
            else4.AddChild(_node42);
            else4.SetHasEvents(else4.HasEvents() | _node42.HasEvents());
            Sequence sequence10 = new Sequence();
            sequence10.SetClassNameString("Sequence");
            sequence10.SetId(0x79);
            else4.AddChild(sequence10);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node24 _node43 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node24();
            _node43.SetClassNameString("Action");
            _node43.SetId(0x18);
            sequence10.AddChild(_node43);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node43.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node444 _node44 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node444();
            _node44.SetClassNameString("Action");
            _node44.SetId(0x1bc);
            sequence10.AddChild(_node44);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node44.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node122 _node45 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node122();
            _node45.SetClassNameString("Assignment");
            _node45.SetId(0x7a);
            sequence10.AddChild(_node45);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node45.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node123 _node46 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node123();
            _node46.SetClassNameString("Action");
            _node46.SetId(0x7b);
            sequence10.AddChild(_node46);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node46.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node124 _node47 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node124();
            _node47.SetClassNameString("Action");
            _node47.SetId(0x7c);
            sequence10.AddChild(_node47);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node47.HasEvents());
            else4.SetHasEvents(else4.HasEvents() | sequence10.HasEvents());
            Sequence sequence11 = new Sequence();
            sequence11.SetClassNameString("Sequence");
            sequence11.SetId(0x7d);
            else4.AddChild(sequence11);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node126 _node48 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node126();
            _node48.SetClassNameString("Action");
            _node48.SetId(0x7e);
            sequence11.AddChild(_node48);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node48.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node127 _node49 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node127();
            _node49.SetClassNameString("Action");
            _node49.SetId(0x7f);
            sequence11.AddChild(_node49);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node49.HasEvents());
            else4.SetHasEvents(else4.HasEvents() | sequence11.HasEvents());
            sequence9.SetHasEvents(sequence9.HasEvents() | else4.HasEvents());
            precondition4.SetHasEvents(precondition4.HasEvents() | sequence9.HasEvents());
            loop2.SetHasEvents(loop2.HasEvents() | precondition4.HasEvents());
            _node7.SetHasEvents(_node7.HasEvents() | loop2.HasEvents());
            sequence2.SetHasEvents(sequence2.HasEvents() | _node7.HasEvents());
            precondition.SetHasEvents(precondition.HasEvents() | sequence2.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition.HasEvents());
            WithPrecondition precondition5 = new WithPrecondition();
            precondition5.SetClassNameString("WithPrecondition");
            precondition5.SetId(0x203);
            loop.AddChild(precondition5);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node516 _node50 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node516();
            _node50.SetClassNameString("Condition");
            _node50.SetId(0x204);
            precondition5.AddChild(_node50);
            precondition5.SetHasEvents(precondition5.HasEvents() | _node50.HasEvents());
            Selector selector2 = new Selector();
            selector2.SetClassNameString("Selector");
            selector2.SetId(0x22);
            precondition5.AddChild(selector2);
            Sequence sequence12 = new Sequence();
            sequence12.SetClassNameString("Sequence");
            sequence12.SetId(0x2b);
            selector2.AddChild(sequence12);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node25 _node51 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node25();
            _node51.SetClassNameString("Condition");
            _node51.SetId(0x19);
            sequence12.AddChild(_node51);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node51.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node45 _node52 = new Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node45();
            _node52.SetClassNameString("Assignment");
            _node52.SetId(0x2d);
            sequence12.AddChild(_node52);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node52.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node29 _node53 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node29();
            _node53.SetClassNameString("Action");
            _node53.SetId(0x1d);
            sequence12.AddChild(_node53);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node53.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node30 _node54 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node30();
            _node54.SetClassNameString("Action");
            _node54.SetId(30);
            sequence12.AddChild(_node54);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node54.HasEvents());
            selector2.SetHasEvents(selector2.HasEvents() | sequence12.HasEvents());
            Sequence sequence13 = new Sequence();
            sequence13.SetClassNameString("Sequence");
            sequence13.SetId(0x205);
            selector2.AddChild(sequence13);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node31 _node55 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node31();
            _node55.SetClassNameString("Condition");
            _node55.SetId(0x1f);
            sequence13.AddChild(_node55);
            sequence13.SetHasEvents(sequence13.HasEvents() | _node55.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node518 _node56 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node518();
            _node56.SetClassNameString("Action");
            _node56.SetId(0x206);
            sequence13.AddChild(_node56);
            sequence13.SetHasEvents(sequence13.HasEvents() | _node56.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node519 _node57 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node519();
            _node57.SetClassNameString("Action");
            _node57.SetId(0x207);
            sequence13.AddChild(_node57);
            sequence13.SetHasEvents(sequence13.HasEvents() | _node57.HasEvents());
            IfElse else5 = new IfElse();
            else5.SetClassNameString("IfElse");
            else5.SetId(520);
            sequence13.AddChild(else5);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node521 _node58 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node521();
            _node58.SetClassNameString("Condition");
            _node58.SetId(0x209);
            else5.AddChild(_node58);
            else5.SetHasEvents(else5.HasEvents() | _node58.HasEvents());
            Sequence sequence14 = new Sequence();
            sequence14.SetClassNameString("Sequence");
            sequence14.SetId(0x20a);
            else5.AddChild(sequence14);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node523 _node59 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node523();
            _node59.SetClassNameString("Action");
            _node59.SetId(0x20b);
            sequence14.AddChild(_node59);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node59.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node524 _node60 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node524();
            _node60.SetClassNameString("Action");
            _node60.SetId(0x20c);
            sequence14.AddChild(_node60);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node60.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node526 _node61 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node526();
            _node61.SetClassNameString("Action");
            _node61.SetId(0x20e);
            sequence14.AddChild(_node61);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node61.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node32 _node62 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node32();
            _node62.SetClassNameString("Action");
            _node62.SetId(0x20);
            sequence14.AddChild(_node62);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node62.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node530 _node63 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node530();
            _node63.SetClassNameString("Action");
            _node63.SetId(530);
            sequence14.AddChild(_node63);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node63.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node531 _node64 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node531();
            _node64.SetClassNameString("DecoratorLoopUntil");
            _node64.SetId(0x213);
            sequence14.AddChild(_node64);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node532 _node65 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node532();
            _node65.SetClassNameString("Condition");
            _node65.SetId(0x214);
            _node64.AddChild(_node65);
            _node64.SetHasEvents(_node64.HasEvents() | _node65.HasEvents());
            sequence14.SetHasEvents(sequence14.HasEvents() | _node64.HasEvents());
            else5.SetHasEvents(else5.HasEvents() | sequence14.HasEvents());
            Sequence sequence15 = new Sequence();
            sequence15.SetClassNameString("Sequence");
            sequence15.SetId(0x215);
            else5.AddChild(sequence15);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node33 _node66 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node33();
            _node66.SetClassNameString("Action");
            _node66.SetId(0x21);
            sequence15.AddChild(_node66);
            sequence15.SetHasEvents(sequence15.HasEvents() | _node66.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node535 _node67 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node535();
            _node67.SetClassNameString("Action");
            _node67.SetId(0x217);
            sequence15.AddChild(_node67);
            sequence15.SetHasEvents(sequence15.HasEvents() | _node67.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node536 _node68 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node536();
            _node68.SetClassNameString("DecoratorLoopUntil");
            _node68.SetId(0x218);
            sequence15.AddChild(_node68);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node537 _node69 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node537();
            _node69.SetClassNameString("Condition");
            _node69.SetId(0x219);
            _node68.AddChild(_node69);
            _node68.SetHasEvents(_node68.HasEvents() | _node69.HasEvents());
            sequence15.SetHasEvents(sequence15.HasEvents() | _node68.HasEvents());
            else5.SetHasEvents(else5.HasEvents() | sequence15.HasEvents());
            sequence13.SetHasEvents(sequence13.HasEvents() | else5.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node538 _node70 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node538();
            _node70.SetClassNameString("Action");
            _node70.SetId(0x21a);
            sequence13.AddChild(_node70);
            sequence13.SetHasEvents(sequence13.HasEvents() | _node70.HasEvents());
            selector2.SetHasEvents(selector2.HasEvents() | sequence13.HasEvents());
            precondition5.SetHasEvents(precondition5.HasEvents() | selector2.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition5.HasEvents());
            WithPrecondition precondition6 = new WithPrecondition();
            precondition6.SetClassNameString("WithPrecondition");
            precondition6.SetId(0x15);
            loop.AddChild(precondition6);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node22 _node71 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node22();
            _node71.SetClassNameString("Condition");
            _node71.SetId(0x16);
            precondition6.AddChild(_node71);
            precondition6.SetHasEvents(precondition6.HasEvents() | _node71.HasEvents());
            Sequence sequence16 = new Sequence();
            sequence16.SetClassNameString("Sequence");
            sequence16.SetId(0x1b);
            precondition6.AddChild(sequence16);
            Selector selector3 = new Selector();
            selector3.SetClassNameString("Selector");
            selector3.SetId(0x1e7);
            sequence16.AddChild(selector3);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node488 _node72 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node488();
            _node72.SetClassNameString("Action");
            _node72.SetId(0x1e8);
            selector3.AddChild(_node72);
            selector3.SetHasEvents(selector3.HasEvents() | _node72.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node35 _node73 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node35();
            _node73.SetClassNameString("Action");
            _node73.SetId(0x23);
            selector3.AddChild(_node73);
            selector3.SetHasEvents(selector3.HasEvents() | _node73.HasEvents());
            sequence16.SetHasEvents(sequence16.HasEvents() | selector3.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node65 _node74 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node65();
            _node74.SetClassNameString("DecoratorLoop");
            _node74.SetId(0x41);
            sequence16.AddChild(_node74);
            Noop noop2 = new Noop();
            noop2.SetClassNameString("Noop");
            noop2.SetId(0x42);
            _node74.AddChild(noop2);
            _node74.SetHasEvents(_node74.HasEvents() | noop2.HasEvents());
            sequence16.SetHasEvents(sequence16.HasEvents() | _node74.HasEvents());
            precondition6.SetHasEvents(precondition6.HasEvents() | sequence16.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition6.HasEvents());
            WithPrecondition precondition7 = new WithPrecondition();
            precondition7.SetClassNameString("WithPrecondition");
            precondition7.SetId(450);
            loop.AddChild(precondition7);
            Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node454 _node75 = new Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node454();
            _node75.SetClassNameString("Condition");
            _node75.SetId(0x1c6);
            precondition7.AddChild(_node75);
            precondition7.SetHasEvents(precondition7.HasEvents() | _node75.HasEvents());
            Sequence sequence17 = new Sequence();
            sequence17.SetClassNameString("Sequence");
            sequence17.SetId(0x1c7);
            precondition7.AddChild(sequence17);
            Action_bt_WrapperAI_Soldier_BTSoldierNormal_node456 _node76 = new Action_bt_WrapperAI_Soldier_BTSoldierNormal_node456();
            _node76.SetClassNameString("Action");
            _node76.SetId(0x1c8);
            sequence17.AddChild(_node76);
            sequence17.SetHasEvents(sequence17.HasEvents() | _node76.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node457 _node77 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierNormal_node457();
            _node77.SetClassNameString("DecoratorLoop");
            _node77.SetId(0x1c9);
            sequence17.AddChild(_node77);
            Noop noop3 = new Noop();
            noop3.SetClassNameString("Noop");
            noop3.SetId(0x1ca);
            _node77.AddChild(noop3);
            _node77.SetHasEvents(_node77.HasEvents() | noop3.HasEvents());
            sequence17.SetHasEvents(sequence17.HasEvents() | _node77.HasEvents());
            precondition7.SetHasEvents(precondition7.HasEvents() | sequence17.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition7.HasEvents());
            _node2.SetHasEvents(_node2.HasEvents() | loop.HasEvents());
            pChild.SetHasEvents(pChild.HasEvents() | _node2.HasEvents());
            bt.SetHasEvents(bt.HasEvents() | pChild.HasEvents());
            return true;
        }
    }
}

