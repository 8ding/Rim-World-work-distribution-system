using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AI : MonoBehaviour
{

    public Transform Target;

    public float Speed = 0;

    public float NextWayPointDistance = 3f;

    private Path path;

    private int currentPoint = 0;

    private bool reachedEndOfPath = false;

    private Seeker seeker;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    private void UpdatePath()
    {
        seeker.StartPath(rb.position, Target.position, OnPathComplete);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
    }

    void OnPathComplete(Path _path)
    {
        if(!_path.error)
        {
            path = _path;
            currentPoint = 0;
        }
    }

    void move()
    {
        if(path == null)
            return;
        if(currentPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }
        Vector2 direction = ((Vector2) path.vectorPath[currentPoint] - rb.position).normalized;

        rb.AddForce(direction * Speed * Time.fixedDeltaTime);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentPoint]);

        if(distance < NextWayPointDistance)
        {
            currentPoint++;
        }
    }
}
