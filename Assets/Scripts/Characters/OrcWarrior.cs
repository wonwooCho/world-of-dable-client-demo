using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcWarrior : CharacterBase
{
    protected override string ANIM_KEY_IDLE => "OrcWarrior_Idle";
    protected override string ANIM_KEY_WALK => "OrcWarrior_Walk";
    protected override string ANIM_KEY_ATTACK => "OrcWarrior_Attack";
    protected override string ANIM_KEY_DIE => "OrcWarrior_Death";
}
