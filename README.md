# UnityChaser
Multi-agent Reinforcement Learning Environment

A Unity ML-Agents based simulation environment that considers situations where multiple agents may have a common or conflicting purpose.

<a href="https://github.com/Unity-Technologies/ml-agents">Unity ML-Agents</a>

## Documentation
<ul>
  The name of this simulation environment is Chaser, which is an environment that considers the situation where 1~N red balls chase 1~N blue balls in MxM size map.
  When the red ball hits a blue ball, the blue ball disappears immediately. The game progresses until every blue ball disappear.
  

## State
<li>
Visual Observations
</li>
    Continuous image data from a camera attached to the ball
  
<li>
Vector Observations
</li>
    The vector distance difference between the 3D positions of the obstacles and different colored balls that are less than a certain distance within the viewing angle of the ball from the 3D position of the ball
  
  
  
## Action
<li>
Continuous action space
</li>
<li>
An action is represented by [x_torque, y_torque]
</li>
    One torque on each of the x and z axes into the [-1, 1] range


## Reward
<li>
Red Ball's Reward
</li>
    If it catches the blue ball, it gains +1 reward.
    It gains a reward of -0.001 every step.
    
<li>
Blue Ball's Reward - Preparing...
</li>
    If it disappears by a red ball, it gains -1 reward.
    It gains a reward of +0.001 every step.
    
</ul>

