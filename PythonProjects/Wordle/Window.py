import pygame
from pygame.locals import *
from Screen import Screen

class Window(Screen):
	def __init__(self, size, caption, backgroundColor):

		Screen.__init__(self, size)
		self.display = pygame.display.set_mode(self.size)
		pygame.display.set_caption(caption)
		self.display.fill(backgroundColor)

		self.backgroundColor = backgroundColor
		
		pygame.display.update()

	def clear(self):
		self.display.fill(self.backgroundColor)