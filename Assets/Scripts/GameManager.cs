using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;

namespace Assets.Scripts
{
    public enum Direction { LEFT, RIGHT, UP, DOWN }

    public class GameManager : Singleton<GameManager>
    {
        int maxCol = 5;
        int maxRow = 5;

        public List<BoundaryCheck> m_barriers = null;

        private Dictionary<Vector2, Marker> m_Markers = null;
        /// <summary>
        /// Initialized in CreateMakerDictionary
        /// Updated by the OnTriggerEnter in Marker.
        /// </summary>
        public Marker CurrentMarker { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            CreateMarkerDictionary();
            CreateNeighbors();

            CreateBarriersList();
        }

        private void CreateBarriersList()
        {
            GameObject[] allBarriers =
            GameObject.FindGameObjectsWithTag("Barrier");

            m_barriers = new List<BoundaryCheck>();
            foreach (GameObject barrier in allBarriers)
            {

                BoundaryCheck boundaryCheckComponent =
                barrier.GetComponent<BoundaryCheck>();
                m_barriers.Add(boundaryCheckComponent);
            }
        }


        /// <summary
        /// Add all of the markers to m_Markers dictionary
        /// which is used to create the neighbors for each marker
        /// in CreateNeighbors.
        /// </summary>
        private void CreateMarkerDictionary()
        {
            GameObject[] allMarkers =
            GameObject.FindGameObjectsWithTag("Marker");

            m_Markers = new Dictionary<Vector2, Marker>();
            foreach (GameObject marker in allMarkers)
            {

                Marker markerComponent =
                marker.GetComponent<Marker>();
                m_Markers.Add(markerComponent.rowCol, markerComponent);

                // Set the initial CurrentMarker to the
                // Lower Left marker.
                if (markerComponent.rowCol == Vector2.zero)
                {
                    CurrentMarker = markerComponent;
                }

            }
            
            
        }

        void CreateNeighbors()
        {
            List<Dictionary<Direction, Marker>> markersMap = new List<Dictionary<Direction, Marker>>();
            foreach (KeyValuePair<Vector2 , Marker> pair in m_Markers.
                OrderBy(o => o.Value.rowCol.x).ThenBy(o => o.Value.rowCol.y))
            {
                markersMap.Add(CreateNeighbors(pair));
            }


        }

        private Dictionary<Direction, Marker> CreateNeighbors(KeyValuePair<Vector2, Marker> pair)
        {
            Dictionary<Direction, Marker> neighbors = new Dictionary<Direction, Marker>();
            int x = (int)pair.Key.x;
            int y = (int)pair.Key.y;
            if (y > 0)
                neighbors[Direction.LEFT] = m_Markers[new Vector2(x, y - 1)];
            else
                neighbors[Direction.LEFT] = null;

            if (y < maxCol)
                neighbors[Direction.RIGHT] = m_Markers[new Vector2(x, y + 1)];
            else
                neighbors[Direction.RIGHT] = null;

            if (x > 0)
                neighbors[Direction.DOWN] = m_Markers[new Vector2(x - 1, y)];
            else
                neighbors[Direction.DOWN] = null;

            if (x < maxRow)
                neighbors[Direction.UP] = m_Markers[new Vector2(x + 1, y)];
            else
                neighbors[Direction.UP] = null;

            pair.Value.neighbors = neighbors;
            return neighbors;
        }

        // Update is called once per frame
        void Update()
        {

        }

        internal bool IsBarrier(Marker current)
        {
            bool fndsBarrier = false;

            foreach (BoundaryCheck barrierCheck in m_barriers)
            {
                if (barrierCheck.IsWithinBoundaries(current))
                {
                    fndsBarrier = true;
                    break;
                }
            }

            return fndsBarrier;
        }
    }
}