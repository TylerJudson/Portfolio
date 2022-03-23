from typing import Tuple
import pygame
from pygame.locals import *
from Style import Style
from Surface import Surface
		

class Button:
	"""Creates a button on the screen that is clickable and hoverable
	"""
	def __init__(self, pos: Tuple[int, int], surface: Surface, style: Style, hoverStyle: Style=None):
		"""Initializes the Button
			Args:
				pos (Tuple[int, int]): The position of the button on the screen
				surface (Surface): The surface of the button used to display
				style (Style): The style of the button used to display
				hoverStyle (HoverStyle, optional): The Style the button is when the mouse is hovering over it. Defaults to None.
		"""
		self.pos = pos
		"""The position of the button on the screen"""
		self.surface = surface
		"""The surface of the button used to display"""
		
		self.style = style
		"""The style of the button used to display"""
		self.hoverStyle = hoverStyle
		"""The Style the button is when the mouse is hovering over it"""
		
		self.rect = Rect(self.surface.pos[0], self.surface.pos[1], self.surface.size[0], self.surface.size[1])
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
				pygame.draw.rect(self.surface.display, self.hoverStyle.fillColor, self.rect, 0, self.hoverStyle.borderRadius)

			# makes the border
			if (self.hoverStyle.borderColor != None):
				pygame.draw.rect(self.surface.display, self.hoverStyle.borderColor, self.rect,
								 self.hoverStyle.borderWidth, self.hoverStyle.borderRadius)

			# render the text on the screen
			if (self.hoverStyle.text != None):
				self.surface.display.blit(self.hoverStyle.text.display, self.hoverStyle.text.rect)

			
		# Else draw the button normally
		else:
			# fills the button
			if (self.style.fillColor != None):
				pygame.draw.rect(self.surface.display, self.style.fillColor,
								 self.rect, 0, self.style.borderRadius)

			# makes the border
			if (self.style.borderColor != None):
				pygame.draw.rect(self.surface.display, self.style.borderColor,
								 self.rect, self.style.borderWidth, self.style.borderRadius)

			# render the text on the screen
			if (self.style.text != None):
				self.surface.display.blit(self.style.text.display, self.style.text.rect)



	def mouseIsHovering(self, mousePos: Tuple[int, int]) -> bool:
		"""Determines whether the mouse is over the button or not

		Args:
			mousePos (Tuple[int, int]): The position of the mouse on the screen

		Returns:
			bool: Whether the mouse is over the button or not
		"""

		# If the mouse pos is inside of the button
		if (self.pos[0] <= mousePos[0] <= self.pos[0] + self.surface.size[0] and self.pos[1] <= mousePos[1] <= self.pos[1] + self.surface.size[1]):
			return True

		return False