using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cirrus.DungeonHealer.World.Objects.Characters.Animations
{
    public enum CharacterAnimation
    {
        Climb=-1670875122,
        Dash=995914302,
        Airborne=-910879650,
        Collider=860563329,
        Blocking_NoBlocking=-1072808265,
        Blocking_Locomotion=541725945,
        Blocking_Idle=1056616440,
        Action_Idle=-1939132686,
        Action_Weapon_OneHanded_RightSwing1=2129291024,
        Action_Item_Drink=-461099035,
        Action_Item_Use=-309469897,
        Action_Item_Eat=-470864282,
        Action_Talk=-815051172,
        Action_Emote1=-2024649953,
        Action_Item_Give=-574313816,
        Reaction_Hurt=-1400209818,
        Reaction_Healed=-1477473801,
        Reaction_Recovered=654821321,
        Reaction_Killed=383897221,
        Reaction_Injured=765498429,
        Reaction_Buffed=-153845636,
        Reaction_Item_Receive=-342666329,
        Action_Pickup0=-269679076,
        Action_Weapon_OneHanded_Swing1=1463361195,
    }
    public interface ICharacterAnimatorWrapper
    {
        float GetStateSpeed(CharacterAnimation state);
        void Play(CharacterAnimation animation, float normalizedTime);
        void Play(CharacterAnimation animation);
        float Speed { set; }
        float Strafe { set; }
        float Health { set; }
        float Direction { set; }
        void Jump();
        void BecomeAirborne();
        void Dash();
        void Climb();
        void Land();
        bool IsGrounded { set; }
        void Injure();
        bool IsInjured { set; }
        bool IsBlocking { set; }
        void EndAction();
        float ActionSpeed { set; }
        float BaseLayerWeight{set;}
        float BlockingLayerWeight{set;}
        float ActionsLayerWeight{set;}
    }
    public class CharacterAnimatorWrapper : ICharacterAnimatorWrapper
    {
        private Animator _animator;
        private Dictionary<CharacterAnimation,float> _stateSpeedValues = new Dictionary<CharacterAnimation,float>();
        public void Play(CharacterAnimation animation, float normalizedTime)
        {
            if(_animator != null)_animator.Play((int)animation, -1, normalizedTime);
        }
        public void Play(CharacterAnimation animation)
        {
            if(_animator != null)_animator.Play((int)animation);
        }
        public float Speed{ set { if(_animator != null)_animator.SetFloat(-823668238, value); } }
        public float Strafe{ set { if(_animator != null)_animator.SetFloat(-1311372029, value); } }
        public float Health{ set { if(_animator != null)_animator.SetFloat(-915003867, value); } }
        public float Direction{ set { if(_animator != null)_animator.SetFloat(-1128574192, value); } }
        public void Jump(){ if(_animator != null)_animator.SetTrigger(125937960); }
        public void BecomeAirborne(){ if(_animator != null)_animator.SetTrigger(1613397964); }
        public void Dash(){ if(_animator != null)_animator.SetTrigger(995914302); }
        public void Climb(){ if(_animator != null)_animator.SetTrigger(-1670875122); }
        public void Land(){ if(_animator != null)_animator.SetTrigger(137525990); }
        public bool IsGrounded{ set { if(_animator != null)_animator.SetBool(507951781, value); } }
        public void Injure(){ if(_animator != null)_animator.SetTrigger(-1712914348); }
        public bool IsInjured{ set { if(_animator != null)_animator.SetBool(-540421922, value); } }
        public bool IsBlocking{ set { if(_animator != null)_animator.SetBool(-887301536, value); } }
        public void EndAction(){ if(_animator != null)_animator.SetTrigger(959881374); }
        public float ActionSpeed{ set { if(_animator != null)_animator.SetFloat(1673499952, value); } }
        public float BaseLayerWeight{set { if(_animator != null) _animator.SetLayerWeight(0,value);} }
        public float BlockingLayerWeight{set { if(_animator != null) _animator.SetLayerWeight(1,value);} }
        public float ActionsLayerWeight{set { if(_animator != null) _animator.SetLayerWeight(2,value);} }
        public CharacterAnimatorWrapper(Animator animator)
        {
            _animator = animator;
            _stateSpeedValues.Add(CharacterAnimation.Climb,1);
            _stateSpeedValues.Add(CharacterAnimation.Dash,1);
            _stateSpeedValues.Add(CharacterAnimation.Airborne,1);
            _stateSpeedValues.Add(CharacterAnimation.Collider,1);
            _stateSpeedValues.Add(CharacterAnimation.Blocking_NoBlocking,1);
            _stateSpeedValues.Add(CharacterAnimation.Blocking_Locomotion,1);
            _stateSpeedValues.Add(CharacterAnimation.Blocking_Idle,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Idle,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Weapon_OneHanded_RightSwing1,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Item_Drink,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Item_Use,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Item_Eat,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Talk,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Emote1,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Item_Give,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Hurt,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Healed,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Recovered,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Killed,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Injured,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Buffed,1);
            _stateSpeedValues.Add(CharacterAnimation.Reaction_Item_Receive,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Pickup0,1);
            _stateSpeedValues.Add(CharacterAnimation.Action_Weapon_OneHanded_Swing1,1);
        }
        public float GetStateSpeed(CharacterAnimation state)
        {
            if(_stateSpeedValues.TryGetValue(state, out float res)) return res;
            return -1f;
        }
        public float GetClipLength(CharacterAnimation state)
        {
            return -1f;
        }
    }
}
