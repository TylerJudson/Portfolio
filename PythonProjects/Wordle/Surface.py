import pygame
from pygame.locals import *
from Screen import Screen

class Surface(Screen):
	def __init__(self, size, backgroundColor):
		Screen.__init__(self, size)

		self.backgroundColor = backgroundColor
		self.display = pygame.Surface(size)
		self.display.fill(backgroundColor)

	def clear(self):
		self.display.fill(self.backgroundColor)