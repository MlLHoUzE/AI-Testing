using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager{
    

    private Vector3 steering;
    private GameObject host;
    private EnemySettings hostScript;

    private double wanderAngle;
    private float ANGLE_CHANGE = 1.0f;

    private List<Collider> walls = new List<Collider>();
	
    //call this function on creating the SteeringManager
    public void Initiate(GameObject host, EnemySettings script)
    {
        this.host = host;
        this.steering = new Vector3(0, 0, 0);
        hostScript = script;

        GameObject[]wallsObjects = GameObject.FindGameObjectsWithTag("Wall");
        for(int i = 0; i < wallsObjects.Length; i++)
        {
            walls.Add(wallsObjects[i].GetComponent<Collider>());
        }
    }
    
    public void ClearSteering()
    {
        steering = new Vector3(0, 0, 0);
    }

    public void Seek(Vector3 targetPosition)
    {
        steering += doSeek(targetPosition);
    }

    private Vector3 doSeek(Vector3 targetPosition)
    {


        Vector3 desiredVelocity = (targetPosition - host.transform.position);// * maxVelocity;
        desiredVelocity.y = 0;
        desiredVelocity = Vector3.Normalize(desiredVelocity) * hostScript.getMaxVelocity();
        Vector3 force = desiredVelocity - hostScript.getVelocity();
        force.y = 0;
        return force;
    }

    public void Flee(Vector3 targetPosition)
    {
        steering += doFlee(targetPosition);
    }

    private Vector3 doFlee(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = (host.transform.position - targetPosition);// * maxVelocity;
        desiredVelocity.y = 0;
        desiredVelocity = Vector3.Normalize(desiredVelocity) * hostScript.getMaxVelocity();
        Vector3 force = desiredVelocity - hostScript.getVelocity();
        force.y = 0;
        return force;
    }

    public void Arrive(Vector3 targetPosition)
    {
        steering += doArrive(targetPosition);
    }

    private Vector3 doArrive(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = (targetPosition - host.transform.position);// * maxVelocity;
        desiredVelocity.y = 0;
        float distance = desiredVelocity.magnitude;

        Vector3 force = new Vector3(0, 0, 0);

        float outerRadius = hostScript.getOuterRadius();
        float innerRadius = hostScript.getInnerRadius();

        if (distance > outerRadius)
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * hostScript.getMaxVelocity();
            force = desiredVelocity - hostScript.getVelocity();
        }
        else if (distance < innerRadius)
        {
            desiredVelocity = Vector3.Normalize(-desiredVelocity) * hostScript.getMaxVelocity();
            force = desiredVelocity - hostScript.getVelocity();
        }
        else
        {
            float distanceIntoZone = distance - innerRadius;
            float zoneSize = outerRadius - innerRadius;
            float midwayPoint = zoneSize / 2;
            float percentage;
            if (distanceIntoZone > midwayPoint - 0.5f && distanceIntoZone < midwayPoint + 0.5f)
            {
                //stop the object
                
                desiredVelocity = new Vector3(0, 0, 0);
                if (hostScript.getVelocity().magnitude > 0)
                {
                    host.GetComponent<Rigidbody>().velocity = desiredVelocity;
                }
            }
            else if (distanceIntoZone > midwayPoint)
            {
                percentage = distanceIntoZone / zoneSize;
                percentage += 1;
                desiredVelocity /= percentage;
                force = desiredVelocity - hostScript.getVelocity();
            }
            else if (distanceIntoZone < midwayPoint)
            {
                distanceIntoZone -= midwayPoint;
                percentage = distanceIntoZone / midwayPoint;
                desiredVelocity *= percentage;
                force = desiredVelocity - hostScript.getVelocity();
            }
        }
        
        force.y = 0;
        return force;
    }

    public void Pursue(Vector3 targetPosition, Vector3 targetVelocity)
    {
        steering += doPursue(targetPosition, targetVelocity);
    }

    private Vector3 doPursue(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float predictionDistance = (targetPosition - host.transform.position).magnitude/hostScript.getMaxVelocity();
        Vector3 futurePosition = targetPosition + targetVelocity * predictionDistance;
        return doSeek(futurePosition);

        
    }

    public void Evade(Vector3 targetPosition, Vector3 targetVelocity)
    {
        steering += doEvade(targetPosition, targetVelocity);
    }

    private Vector3 doEvade(Vector3 targetPosition, Vector3 targetVelocity)
    {
        float predictionDistance = (targetPosition - host.transform.position).magnitude / hostScript.getMaxVelocity();
        Vector3 futurePosition = targetPosition + targetVelocity * predictionDistance;
        return doFlee(futurePosition);
    }

    public void Wander()
    {
        steering += doWander();
    }

    private Vector3 doWander()
    {
        //calculate circle center
        Vector3 circleCenter;
        circleCenter = hostScript.getVelocity();
        circleCenter.Normalize();
        circleCenter *= hostScript.getCircleDistance();


        Vector3 displacement;
        displacement = new Vector3(0, 0, -1);
        displacement *= hostScript.getCircleRadius();

        
        //randomly change the vector direction
        displacement = setAngle(displacement, wanderAngle);

        wanderAngle += (Random.value * ANGLE_CHANGE) - (ANGLE_CHANGE * 0.5);

        Vector3 wanderForce;
        wanderForce = circleCenter + displacement;
        return wanderForce;
    }

    public void wallAvoid(Vector3 targetPos)
    {
        steering += doWallAvoid(targetPos);
    }

    private Vector3 doWallAvoid(Vector3 targetPos)
    {
        Transform[] mFeelers = hostScript.getFeelers();

        float distToThisIP = 0.0f;
        float distToClosestIP = float.MaxValue;
        
        

        //this will hold an index into the vector of walls
        int closestWall = -1;

        Vector3 steeringForce,
                point,          //used for storing temporary info
                closestPoint;   //holds the closest intersecting point

        closestPoint = new Vector3(0, 0, 0);
        steeringForce = new Vector3(0, 0, 0);

        //examine each feeler in turn
        for (int flr = 0; flr < mFeelers.Length; ++flr)
        {
            //run through each wall checking for any intersection points
            for(int w = 0; w < walls.Count; ++w)
            {
                if(LineIntersectionPoint(host.transform.position, mFeelers[flr].position, walls[w].bounds.min, walls[w].bounds.max, out point, out distToThisIP))
                {
                    //is this the closest intersection point
                    if(distToThisIP < distToClosestIP)
                    {
                        distToClosestIP = distToThisIP;

                        closestWall = w;

                        closestPoint = point;
                    }
                }
            }//next wall

            //if an intersection point has been detected, calculate a force that will direct the agent away
            if(closestWall >= 0)
            {

                Debug.Log("Wall Detected");
                //calculate by what distance the projected position of the agent will overshoot the wall
                Vector3 overShoot = mFeelers[flr].position - closestPoint;

                //create a force in the direction of the wall normal, with a magnitude of the overshoot
                Vector3 wallNormal = calcWallNormal(walls[closestWall]);

                //check vertical or horizontal wall

                

                if(wallNormal.x < 0 && (targetPos - host.transform.position).x < 0)
                {
                    //flip normal
                    wallNormal *= -1;
                }
                else if(wallNormal.z < 0 && (targetPos - host.transform.position).z < 0)
                    {
                    wallNormal *= -1;
                }

                steeringForce = wallNormal * overShoot.magnitude * hostScript.getMaxVelocity();

            }
        }//next feeler
        return steeringForce;
    }

    public Vector3 getSteeringValue(float maxForce)
    {
        steering = Vector3.ClampMagnitude(steering, maxForce);
        //potentially have it affected by objects mass

        return steering;
    }

    private Vector3 setAngle(Vector3 vector, double value)
    {
        float length = vector.magnitude;
        vector.x = Mathf.Cos((float)value) * length;
        vector.z = Mathf.Sin((float)value) * length;
        return vector;
    }

    private bool LineIntersectionPoint(Vector3 ps1, Vector3 pe1, Vector3 ps2, Vector3 pe2, out Vector3 point, out float distToThisIP)
    {
        //Vector3 r = pe1 - ps1;
        //Vector3 s = pe2 - ps2;

        //float rxs = r.x * s.y - r.y * s.x;
        //float qpxr = (ps2 - ps1).x * r.y - (ps2 - ps1).y * r.x;
        //point = Vector3.zero;
        //distToThisIP = 0;

        ////if r x s = 0 and (ps2-ps1) x r = 0, then the two lines are collinear.
        //if (rxs == 0 && qpxr == 0)
        //{
        //    return false;
        //}

        //if (rxs == 0 && qpxr != 0)
        //    return false;
        //float t = ((ps2 - ps1).x * s.y - (ps2 - ps1).y * s.x) / rxs;
        //float u = ((ps2 - ps1).x * r.y - (ps2 - ps1).y * r.x) / rxs;

        //if (rxs != 0 && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        //{
        //    point = ps1 + t * r;

        //    return true;
        //}
        //return false;
        //float a1 = pe1.z - ps1.z;
        //float b1 = ps1.x - pe1.x;
        //float c1 = a1 * ps1.x + b1 * ps1.z;

        //float a2 = pe2.z - ps2.z;
        //float b2 = ps2.x - pe2.x;
        //float c2 = a2 * ps2.x + b2 * ps2.z;

        //float delta = a1 * b2 - a2 * b1;

        //point = new Vector3((b2 * c1 - b1 * c2) / delta, 0.5f, (a1 * c2 - a2 * c1) / delta);

        //distToThisIP = (point - ps1).magnitude;

        //if (delta == 0)
        //    return false;



        //return true;


        Vector3 CmP = new Vector3(ps2.x - ps1.x, ps2.y - ps1.y, ps2.z - ps1.z);
        Vector3 r2 = new Vector3(pe1.x - ps1.x, pe1.y - ps1.y, pe1.z - ps1.z);
        Vector3 s2 = new Vector3(pe2.x - ps2.x, pe2.y - ps2.y, pe2.z - ps2.z);

        float CmPxr2 = CmP.x * r2.z - CmP.z * r2.x;
        float CmPxs2 = CmP.x * s2.z - CmP.z * s2.x;
        float r2xs2 = r2.x * s2.z - r2.z * s2.x;

        if (CmPxr2 == 0.0f)
        {
            //lines are collinear
            point = pe1;
            distToThisIP = (point - ps1).magnitude;
            return ((ps2.x - ps1.x < 0.0f) != (ps2.x - pe2.x < 0.0f)) || ((ps2.z - ps1.z < 0.0f) != (ps2.z - pe1.z < 0.0f));
        }

        if (r2xs2 == 0.0f)
        {
            point = new Vector3(0, 0, 0);
            distToThisIP = -1;
            return false; //lines are parallel
        }


        float rxsr = 1.0f / r2xs2;
        float t2 = CmPxs2 * rxsr;
        float u2 = CmPxr2 * rxsr;
        point = ps1 + t2 * pe1;
        distToThisIP = (point - ps1).magnitude;
        return (t2 >= 0.0f) && (t2 <= 1.0f) && (u2 >= 0.0f) && (u2 <= 1.0f);
    }

    private Vector3 calcWallNormal(Collider wall)
    {
        Vector3 a = wall.bounds.min; 
        Vector3 b = new Vector3(wall.bounds.center.x + wall.bounds.extents.x, wall.bounds.center.y, wall.bounds.center.z);
        Vector3 c = new Vector3(wall.bounds.center.x, wall.bounds.center.y + wall.bounds.extents.y, wall.bounds.center.z);

        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        Vector3 perp = Vector3.Cross(side2, side1);

        perp.Normalize();

        return perp;
    }
}
