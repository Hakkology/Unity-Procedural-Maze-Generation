using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonClairvoyance : MonoBehaviour
{
    public GameObject particles;
    DungeonMaze thisMaze;
    DungeonPathfinding pathfinding;
    GameObject magic;
    DungeonPathMarker destination;

    void Awake()
    {
        pathfinding = GetComponent<DungeonPathfinding>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pathfinding == null) return;
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -Vector3.up);
            if (Physics.Raycast(ray, out hit))
            {
                thisMaze = hit.collider.gameObject.GetComponentInParent<DungeonMaze>();
                DungeonMapLocation location = hit.collider.gameObject.GetComponentInParent<DungeonMapLocation>();
                MapLocation currentLocation = new MapLocation(location.x, location.z);
                destination = pathfinding.Build(thisMaze, currentLocation, thisMaze.exitPoint);

                magic = Instantiate(particles, transform.position, transform.rotation);
                StartCoroutine(DisplayMagic());

            }
        }
    }

    IEnumerator DisplayMagic()
    {
        List<MapLocation> magicPath = new List<MapLocation>();
        while (destination != null)
        {
            magicPath.Add(new MapLocation(destination.Location.x, destination.Location.z));
            destination = destination.Parent;
        }

        magicPath.Reverse();

        foreach (MapLocation loc in magicPath)
        {
            magic.transform.LookAt(thisMaze.piecePlaces[loc.x, loc.z]._model.transform.position + new Vector3(0, 1, 0));

            int loopTimeout = 0;
            while (Vector2.Distance(
                new Vector2(magic.transform.position.x, magic.transform.position.z),
                new Vector2(thisMaze.piecePlaces[loc.x, loc.z]._model.transform.position.x, thisMaze.piecePlaces[loc.x, loc.z]._model.transform.position.z)) > 2
                && loopTimeout < 100)
            {
                magic.transform.Translate(0, 0, 10f * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
                loopTimeout++;
            }
        }
        Destroy(magic, 10);
    }
}