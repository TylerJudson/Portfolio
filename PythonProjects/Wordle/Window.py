from typing import Tuple
import pygame
from pygame.locals import *
from Screen import Screen

class Window(Screen):
	"""A window used for rendering content
	"""
	def __init__(self, size: Tuple[int, int], caption: str, backgroundColor: Tuple[int, int, int]):
		"""Initializes the window

		Args:
			size (Tuple[int, int]): The size of the screen - size[0] being the width and size[1] being the height
			caption (str): The caption to display on the window
			backgroundColor (Tuple[int, int, int]): The background color for the window
		"""
		Screen.__init__(self, size)
		self.display = pygame.display.set_mode(self.size)
		"""The display of the screen that gets rendered"""

		pygame.display.set_caption(caption)
		self.display.fill(backgroundColor)

		self.backgroundColor = backgroundColor
		"""The background color for the window"""

		pygame.display.update()

	def clear(self):
		"""Clears the window
		"""
		self.display.fill(self.backgroundColor)