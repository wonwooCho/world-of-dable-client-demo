using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footman : CharacterBase
{
    protected override string ANIM_KEY_IDLE => "Footman_Idle";
    protected override string ANIM_KEY_WALK => "Footman_Walk";
    protected override string ANIM_KEY_ATTACK => "Footman_Attack";
    protected override string ANIM_KEY_DIE => "Footman_Death";
}
