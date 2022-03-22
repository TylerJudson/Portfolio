from typing import Tuple

from Text import Text


class HoverStyle:
	"""The styles to be displayed on hover
	"""
	def __init__(self, text: Text, fillColor: Tuple[int, int, int]=(0, 0, 0), borderColor: Tuple[int, int, int]=(0, 0, 0)):
		"""Initializes HoverStyle

		Args:
			text (Text): The text to be displayed on hover
			fillColor (Tuple[int, int, int], optional): The fill color to be displayed on hover. Defaults to (0, 0, 0).
			borderColor (Tuple[int, int, int], optional): The border color to be displayed on hover. Defaults to (0, 0, 0).
		"""
		self.text = text
		"""The text to be displayed on hover"""
		self.fillColor = fillColor
		"""The fill color to be displayed on hover"""
		self.borderColor = borderColor
		"""The border color to be displayed on hover"""