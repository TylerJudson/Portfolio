from typing import Tuple
from Text import Text

class Style:
	"""The Style to be displayed"""
	
	def __init__(self, text: Text=None, fillColor: Tuple[int, int, int]=None, borderColor: Tuple[int, int, int]=None, borderWidth: int=2, borderRadius: int=0):
		"""Initializes the Style object

		Args:
			text (Text, optional): The text to be displayed. Defaults to None.
			fillColor (Tuple[int, int, int], optional): The fill color to be displayed. Defaults to None.
			borderColor (Tuple[int, int, int], optional): The border color to be displayed. Defaults to None.
			borderWidth (int, optional): The border width to be displayed. Defaults to 2
			borderRadius (int, optional): How rounded the corners of the border are. Defaults to 0.
		"""
		self.text = text
		"""The text to be displayed"""

		self.fillColor = fillColor
		"""The fill color to be displayed"""

		self.borderColor = borderColor
		"""The border color to be displayed"""

		self.borderWidth = borderWidth
		"""The width of the border to be displayed"""

		self.borderRadius = borderRadius
		"""How rounded the corners of the border are"""
