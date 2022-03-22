from typing import Tuple
import pygame
from pygame.locals import *
from HoverStyle import HoverStyle
from Surface import Surface
from Text import Text
		

class Button:
	"""Creates a button on the screen that is clickable and hoverable
	"""
	def __init__(self, pos: Tuple[int, int], surface: Surface, text: Text, border: bool=False, borderColor: Tuple[int, int, int]=(0, 0, 0), borderRadius: int=0, hoverStyle: HoverStyle=None):
		"""Initializes the Button
			Args:
				pos (Tuple[int, int]): The position of the button on the screen
				surface (Surface): The surface of the button used to display
				text (Text): The text of the button
				border (bool, optional): Whether or not to display a border. Defaults to False.
				borderColor (Tuple[int, int, int], optional): The border color to display if border is True. Defaults to (0, 0, 0).
				borderRadius (int, optional): How rounded the corners of the border are. Defaults to 0.
				hoverStyle (HoverStyle, optional): The Style the button is when the mouse is hovering over it. Defaults to None.
		"""
		self.pos = pos
		"""The position of the button on the screen"""
		self.surface = surface
		"""The surface of the button used to display"""
		
		self.text = text
		"""The text of the button"""
		
		self.border = border
		"""Whether or not to display a border"""
		self.borderColor = borderColor
		"""The border color to display if border is True"""

		self.borderRadius = borderRadius
		"""How rounded the corners of the border are."""

		self.hoverStyle = hoverStyle
		"""The Style the button is when the mouse is hovering over it"""
		
		self.rect = Rect(self.surface.pos[0], self.surface.pos[0], self.surface.size[0], self.surface.size[1])
		"""The Rect of the button used for rendering"""

	def render(self, mousePos: Tuple[int, int]=(-1, -1)):
		"""Renders the Button

		Args:
			mousePos (Tuple[int, int], optional): Where the mouse is on the screen. Defaults to (-1, -1).
		"""

		# clear the surface
		self.surface.clear()
		# If we want to hover and the mouse is hovering over the button
		# Changes the style of the button
		if (self.hoverStyle != None and self.mouseIsHovering(mousePos)):

			# fills the button
			if (self.hoverStyle.fillColor != None):
				pygame.draw.rect(self.surface.display, self.hoverStyle.fillColor, self.rect, 0, self.borderRadius)

			# makes the border
			if (self.hoverStyle.borderColor != None):
				pygame.draw.rect(self.surface.display, self.hoverStyle.borderColor, self.rect, 2, self.borderRadius)

			# render the text on the screen
			self.surface.display.blit(self.hoverStyle.text.display, self.hoverStyle.text.rect)

			
		# Else draw the button normally
		else:
			# makes the border
			if (self.border):
				pygame.draw.rect(self.surface.display, self.borderColor, self.rect, 2, self.borderRadius)

			# render the text on the screen
			self.surface.display.blit(self.text.display, self.text.rect)

	def mouseIsHovering(self, mousePos: Tuple[int, int]) -> bool:
		"""Determines whether the mouse is over the button or not

		Args:
			mousePos (Tuple[int, int]): The position of the mouse on the screen

		Returns:
			bool: Whether the mouse is over the button or not
		"""

		# If the mouse pos is inside of the 
		if (self.pos[0] <= mousePos[0] <= self.pos[0] + self.surface.size[0] and self.pos[1] <= mousePos[1] <= self.pos[1] + self.surface.size[1]):
			return True

		return False