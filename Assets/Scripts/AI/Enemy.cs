using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour {


    private SteeringManager mSteeringManager;
    private GameObject mTarget;
    private Rigidbody mRigidBody;
    private EnemySettings mSettingsScript;

    // Use this for initialization
    void Start () {
        mSettingsScript = GetComponent<EnemySettings>();
        mSteeringManager = new SteeringManager();
        mSteeringManager.Initiate(this.gameObject, mSettingsScript);
        mTarget = GameObject.FindGameObjectWithTag("Player");
        mRigidBody = GetComponent<Rigidbody>();
        
	}
	
	// Update is called once per frame
	void Update () {

        mSteeringManager.ClearSteering();

        switch(mSettingsScript.getEnemyType())
        {
            case EnemySettings.enemyType.Seek:
                {
                    mSteeringManager.Seek(mTarget.transform.position);
                    mSteeringManager.wallAvoid(mTarget.transform.position);
                    changeColour(Color.cyan);
                    break;
                }
            case EnemySettings.enemyType.Flee:
                {
                    mSteeringManager.Flee(mTarget.transform.position);
                    changeColour(Color.green);
                    break;
                }
            case EnemySettings.enemyType.Arrive:
                {
                    mSteeringManager.Arrive(mTarget.transform.position);
                    changeColour(Color.yellow);
                    break;
                }
            case EnemySettings.enemyType.Pursue:
                {
                    mSteeringManager.Pursue(mTarget.transform.position, mTarget.GetComponent<Rigidbody>().velocity);
                    changeColour(Color.magenta);
                    break;
                }
            case EnemySettings.enemyType.Evade:
                {
                    mSteeringManager.Evade(mTarget.transform.position, mTarget.GetComponent<Rigidbody>().velocity);
                    changeColour(Color.black);
                    break;
                }
            case EnemySettings.enemyType.Wander:
                {
                    mSteeringManager.Wander();
                    mSteeringManager.Arrive(mTarget.transform.position);
                    changeColour(Color.grey);
                    break;
                }
        }

        Vector3 steering = mSteeringManager.getSteeringValue(mSettingsScript.getMaxForce());

        Vector3 velocityIn = mRigidBody.velocity;
        velocityIn.y = 0;

        Vector3 velocity = Vector3.ClampMagnitude(velocityIn + steering, mSettingsScript.getMaxSpeed());
        mRigidBody.velocity = velocity;


        checkState();
	}

    private void checkState()
    {
        switch(mSettingsScript.getEnemyType())
        {
            case EnemySettings.enemyType.Flee:
                {
                    if ((mTarget.transform.position - transform.position).magnitude > 28)
                    {
                        mSettingsScript.setEnemyType(EnemySettings.enemyType.Seek);
                    }
                    break;
                }
        }
    }

    private void changeColour(Color colorIn)
    {
        GetComponent<Renderer>().material.color = colorIn;
    }


    
}
