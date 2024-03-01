using Snowy.Inventory;
using Systems.Actor;
using Systems.FPS_Movement.Scripts;
using Systems.IK.Base;
using UnityEngine;

public class Player : Actor
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] private FpCamera fpCamera;
    [SerializeField] private BaseIK ik;
    
    protected override void Awake()
    {
        base.Awake();
        
        if (!inventory) inventory = GetComponentInChildren<PlayerInventory>();
        if (!animationManager) animationManager = GetComponentInChildren<AnimationManager>();
        if (!fpCamera) fpCamera = GetComponentInChildren<FpCamera>();
        if (!ik) ik = GetComponentInChildren<BaseIK>();
    }
    
    public PlayerInventory Inventory => inventory;
    public AnimationManager AnimationManager => animationManager;
    
    public FpCamera FpCamera => fpCamera;
    public BaseIK IK => ik;
}
