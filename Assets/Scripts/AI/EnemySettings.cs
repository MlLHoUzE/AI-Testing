using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySettings : MonoBehaviour {


    public enum enemyType
    {
        Seek,
        Flee,
        Arrive,
        Pursue,
        Evade,
        Wander
    };

    
    [SerializeField] private float maxVelocity;
    [SerializeField]
    private float maxForce;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float innerRadius;
    [SerializeField]
    private float outerRadius;
    [SerializeField]
    private enemyType eEnemyType;
    [SerializeField]
    private float circleDistance;
    [SerializeField]
    private float circleRadius;
    [SerializeField]
    private Transform[] feelers;

    public float getMaxVelocity()
    {
        return maxVelocity;
    }

    public Vector3 getVelocity()
    {
        return GetComponent<Rigidbody>().velocity;
    }

    public float getOuterRadius()
    {
        return outerRadius;
    }

    public float getInnerRadius()
    {
        return innerRadius;
    }

    public float getMaxForce()
    {
        return maxForce;
    }

    public float getMaxSpeed()
    {
        return maxSpeed;
    }

    public enemyType getEnemyType()
    {
        return eEnemyType;
    }
    public void setEnemyType(enemyType newType)
    {
        eEnemyType = newType;
    }
    public float getCircleDistance()
    {
        return circleDistance;
    }
    public float getCircleRadius()
    {
        return circleRadius;
    }

    public Transform[] getFeelers()
    {
        return feelers;
    }
}
