from typing import Tuple
import pygame
from pygame.locals import *
from Screen import Screen

class Surface(Screen):
	"""A Surface used for rendering content
	"""
	def __init__(self, pos:Tuple[int, int], size: Tuple[int, int], backgroundColor: Tuple[int, int, int]):
		"""Initializes the Surface

		Args:
			pos (Tuple[int, int]): The position of the surface
			size (Tuple[int, int]): The size of the surface - size[0] being the width and size[1] being the height
			backgroundColor (Tuple[int, int, int]): The background color for the surface
		"""
		Screen.__init__(self, size)

		self.pos = pos
		"""The position of the surface"""
		self.backgroundColor = backgroundColor
		"""The background color for the surface"""
		self.display = pygame.Surface(size)
		"""The display of the surface that gets rendered"""
		self.display.fill(backgroundColor)

	def clear(self):
		"""Clears the surface
		"""
		self.display.fill(self.backgroundColor)