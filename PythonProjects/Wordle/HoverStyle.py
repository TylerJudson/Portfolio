from turtle import fillcolor
from typing import Tuple

from Text import Text

class Style:
	"""The Style to be displayed"""
	
	def __init__(self, text: Text, fillColor: Tuple[int, int, int]=None, borderColor: Tuple[int, int, int]=None, borderRadius: int=0):
		"""Initializes the Style object

		Args:
			text (Text): The text to be displayed
			fillColor (Tuple[int, int, int], optional): The fill color to be displayed. Defaults to None.
			borderColor (Tuple[int, int, int], optional): The border color to be displayed. Defaults to None.
			borderRadius (int, optional): How rounded the corners of the border are. Defaults to 0.
		"""
		self.text = text
		"""The text to be displayed"""

		self.fillColor = fillColor
		"""The fill color to be displayed"""

		self.borderColor = borderColor
		"""The border color to be displayed"""

		self.borderRadius = borderRadius
		"""How rounded the corners of the border are"""

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