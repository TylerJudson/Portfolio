from typing import Tuple


class Screen:
	"""A screen used for rendering content
	"""
	def __init__(self, size: Tuple[int, int]):
		"""Initializes the Screen object

			Args:
				size (Tuple[int, int]): The size of the screen
		"""
		self.size = size
		"""The size of the screen - size[0] being the width and size[1] being the height"""

		self.width = size[0]
		"""The width of the screen"""

		self.height = size[1]
		"""The height of the screen"""