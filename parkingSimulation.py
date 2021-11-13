# Model design
#pip install agentpy
import agentpy as ap
import numpy as np

# Visualization
import seaborn as sns

"""## Model Definition"""

class VehicleAgent(ap.Agent):

  """ An agent tha simulates a vehicle """

  def setup(self):
    self.xUbication = 0
    self.yUbication = 0
    self.fuel = 0
    self.status = "looking"
    self.stepsParking = 0

  def movement(self):
    pass
    # Funcion que definira el siguiente movimiento del agente automovil en el espacio
    # Tentativo:
    # self.xUbication = randint(10, 0);
    # self.yUbication = randint(10, 0);

  def park(self):
    self.status = 'parked'

class ParkingAgent(ap.Agent):

  """ An agent that simulates a parking slot """

  def setup(self):
    self.id
    self.availability = True
    self.xUbication = 0
    serf.yUbication = 0

  def setAvailability(self):
    # Condicional en dependencia a los agentes vehiculo
    pass

class CongestionModel(ap.Model):

  """ A simple model of random wealth transfers """

  def setup(self):
    self.vAgents = ap.AgentList(self, self.p.vehicleAgents, VehicleAgent)
    self.pAgents = ap.AgentList(self, self.p.parkingAgents, ParkingAgent)

  def step(self):
    self.vAgents.movement()

  def update(self):
    pass
    #self.record('Traffic', traffic())

  def end(self):
    pass
    #self.agents.record('')

"""## Simulation Run"""

parameters = {
  'vehicleAgents': 100,
  'parkingAgents': 50,
  'steps': 100,
  'seed': 42,
}

model = CongestionModel(parameters)
#results = model.run()
#results