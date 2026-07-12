using UnityEngine;
using TMPro; //TextMeshPro Library
namespace platform_and_gems.GemCollector
{
    public class GemCollector : MonoBehaviour
    {
        public GameObject Score;    // GameObject for displaying the score
        public GameObject win;      // GameObject for displaying the win message
        public int Targetscore = 3;    // The score required to trigger Game Over
        private int Currentscore = 0;   // Starting score
        private TextMeshProUGUI scoreText;    // TextMeshProUGUI component for the score text
        private TextMeshProUGUI winText;      // TextMeshProUGUI component for the win message

        void Start()
        {
            scoreText = Score.GetComponent<TextMeshProUGUI>();
            winText = win.GetComponent<TextMeshProUGUI>();
            UpdateScoreText();
        }

        void UpdateScoreText()
        {
            // Update the score display
            scoreText.text = Currentscore + "/" + Targetscore;
        }

        private void OnTriggerEnter(Collider collision)
        {
            // If a "Gem" is collected
            if (collision.gameObject.CompareTag("Gem"))
            {
                Currentscore++;
                UpdateScoreText();

                // Remove the Gem from the scene
                Destroy(collision.gameObject);

                // If the target score is reached, display the win message
                if (Currentscore >= Targetscore)
                {
                    winText.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    // To pause the game, add: Time.timeScale = 0;
                }
            }
        }
    }
}
