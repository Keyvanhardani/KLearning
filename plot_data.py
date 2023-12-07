import matplotlib.pyplot as plt
import json

# Lade die Daten
with open('rewardsPerEpisode.json', 'r') as file:
    rewards = json.load(file)

# Erstelle den Plot
plt.plot(rewards)
plt.title('Q-Learning Fortschritt')
plt.xlabel('Episode')
plt.ylabel('Belohnung')
plt.savefig('rewards_plot.png')
plt.show()